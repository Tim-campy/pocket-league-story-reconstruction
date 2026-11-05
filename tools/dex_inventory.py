"""Inventory DEX class definitions and packages without decoding any method.

Reads only header, string_ids/string_data, type_ids and class_defs. No class_data,
code_item, instruction, decompiler, class loading or target execution is used.
Ownership labels are explicitly separated from observed class-name inventories.
"""
from collections import Counter, defaultdict
import hashlib
import json
from pathlib import Path
import struct
import zipfile
import zlib
from compiler_probes import UNITY

ROOT = Path(__file__).resolve().parents[1]


class DexNames:
    def __init__(self, data):
        self.data = data
        if len(data) < 112 or data[:8] != b'dex\n035\0':
            raise ValueError('Expected a complete DEX 035 header')
        if self.u32(32) != len(data) or self.u32(36) != 112 or self.u32(40) != 0x12345678:
            raise ValueError('DEX file size, header size or byte order mismatch')
        if hashlib.sha1(data[32:]).digest() != data[12:32]:
            raise ValueError('DEX SHA-1 signature mismatch')
        if zlib.adler32(data[12:]) & 0xffffffff != self.u32(8):
            raise ValueError('DEX Adler-32 checksum mismatch')
        self.string_count, self.string_offset = self.u32(56), self.u32(60)
        self.type_count, self.type_offset = self.u32(64), self.u32(68)
        self.class_count, self.class_offset = self.u32(96), self.u32(100)
        for count, offset, stride in ((self.string_count, self.string_offset, 4),
                                      (self.type_count, self.type_offset, 4),
                                      (self.class_count, self.class_offset, 32)):
            if offset + count * stride > len(data):
                raise ValueError('DEX inventory table outside file')
        self.strings = {}

    def u32(self, offset):
        if offset < 0 or offset + 4 > len(self.data):
            raise ValueError('DEX uint32 outside file')
        return struct.unpack_from('<I', self.data, offset)[0]

    def uleb128(self, offset):
        value = 0
        for index in range(5):
            if offset >= len(self.data):
                raise ValueError('Truncated DEX ULEB128')
            byte = self.data[offset]
            offset += 1
            value |= (byte & 127) << (index * 7)
            if byte < 128:
                return value, offset
        raise ValueError('Invalid DEX ULEB128')

    def string(self, index):
        if index == 0xffffffff:
            return None
        if index < 0 or index >= self.string_count:
            raise ValueError('String index outside string_ids')
        if index in self.strings:
            return self.strings[index]
        offset = self.u32(self.string_offset + index * 4)
        expected_units, cursor = self.uleb128(offset)
        units = []
        while True:
            if cursor >= len(self.data):
                raise ValueError('Unterminated DEX string_data')
            a = self.data[cursor]
            cursor += 1
            if a == 0:
                break
            if a < 128:
                unit = a
            elif a & 0xe0 == 0xc0:
                if cursor >= len(self.data) or self.data[cursor] & 0xc0 != 0x80:
                    raise ValueError('Invalid two-byte DEX MUTF-8')
                unit = ((a & 31) << 6) | (self.data[cursor] & 63)
                cursor += 1
            elif a & 0xf0 == 0xe0:
                if cursor + 1 >= len(self.data) or any(v & 0xc0 != 0x80 for v in self.data[cursor:cursor + 2]):
                    raise ValueError('Invalid three-byte DEX MUTF-8')
                unit = ((a & 15) << 12) | ((self.data[cursor] & 63) << 6) | (self.data[cursor + 1] & 63)
                cursor += 2
            else:
                raise ValueError('Invalid DEX MUTF-8 lead byte')
            units.append(unit)
        if len(units) != expected_units:
            raise ValueError('DEX string UTF-16 length mismatch')
        text = b''.join(struct.pack('<H', unit) for unit in units).decode('utf-16-le', 'surrogatepass')
        self.strings[index] = text
        return text

    def type_name(self, index):
        if index == 0xffffffff:
            return None
        if index < 0 or index >= self.type_count:
            raise ValueError('Type index outside type_ids')
        return self.string(self.u32(self.type_offset + index * 4))

    def classes(self):
        result = []
        for index in range(self.class_count):
            offset = self.class_offset + index * 32
            descriptor = self.type_name(self.u32(offset))
            if not descriptor.startswith('L') or not descriptor.endswith(';'):
                raise ValueError('class_def does not reference a class descriptor')
            binary = descriptor[1:-1]
            package = binary.rsplit('/', 1)[0].replace('/', '.') if '/' in binary else '(default)'
            result.append({'class_def_index': index, 'class_def_file_offset': offset,
                           'type_id_index': self.u32(offset), 'descriptor': descriptor,
                           'binary_name': binary.replace('/', '.'), 'package': package,
                           'access_flags': self.u32(offset + 4),
                           'superclass': self.type_name(self.u32(offset + 8)),
                           'source_file': self.string(self.u32(offset + 16))})
        return result


def classify(package, classes, official_names):
    observed = [entry['binary_name'] for entry in classes]
    exact_names = sorted(set(observed) & official_names)
    if len(exact_names) == len(observed):
        return {'category': 'Unity', 'confidence': 'corroborated-class-inventory',
                'evidence': 'Every declared class name occurs in official Unity 5.3.4f1 Android Mono Release classes.jar.',
                'official_class_name_matches': len(exact_names),
                'binary_identity': 'UNKNOWN; DEX and JVM bytecode were not compared'}
    if package.startswith('android.support.'):
        category, owner = 'SDK de terceiros', 'Android Support Library'
    elif package.startswith('com.google.') or package.startswith('com.android.vending.'):
        category, owner = 'SDK de terceiros', 'Google SDK namespace and named API classes'
    elif package.startswith('com.mnp.'):
        category, owner = 'SDK de terceiros', 'NativePopupsManager/MnpUtils package naming suggests Mobile Native Popups'
    elif package.startswith('kairo.android.plugin'):
        category, owner = 'codigo proprio candidato', 'Kairo vendor namespace; Java bridge names also occur in recovered KairoLibrary C#'
    elif package == 'net.kairosoft.android.soccer_en':
        category, owner = 'codigo proprio candidato', 'Application package; all declared classes are Android generated R/R$* resource identifiers'
    elif package == 'main' or package == 'plugin' or package.startswith('plugin.'):
        category, owner = 'codigo proprio candidato', 'Application Main and plugin namespaces; plugin.Config/Utility/WebBox are named by recovered Assembly-CSharp bridges'
    else:
        return {'category': 'UNKNOWN', 'confidence': 'UNKNOWN', 'evidence': 'Unrecognized package; no ownership claim'}
    return {'category': category, 'confidence': 'inferred', 'owner_candidate': owner,
            'evidence': 'Observed package and class names, superclass and source_file metadata only; no method examined.',
            'historical_ownership': 'UNKNOWN', 'exact_sdk_version': 'UNKNOWN'}


def main():
    path = ROOT / 'original/extracted/classes.dex'
    data = path.read_bytes()
    dex = DexNames(data)
    classes = dex.classes()
    counts = Counter(entry['descriptor'] for entry in classes)
    if any(count > 1 for count in counts.values()):
        raise ValueError('Duplicate class_def descriptor')
    jar = UNITY / 'Editor/Data/PlaybackEngines/AndroidPlayer/Variations/mono/Release/Classes/classes.jar'
    with zipfile.ZipFile(jar) as archive:
        official_names = {name[:-6].replace('/', '.') for name in archive.namelist() if name.endswith('.class')}
    grouped = defaultdict(list)
    for entry in classes:
        grouped[entry['package']].append(entry)
    packages = []
    for package, entries in sorted(grouped.items()):
        packages.append({'package': package, 'class_count': len(entries),
                         'classification': classify(package, entries, official_names), 'classes': entries})
    totals = Counter()
    for package in packages:
        totals[package['classification']['category']] += package['class_count']
    report = {'schema_version': 1, 'operation': 'class-definition-table-inventory-only',
              'artifact': {'path': str(path.relative_to(ROOT)), 'byte_length': len(data),
                           'sha256': hashlib.sha256(data).hexdigest(), 'dex_version': '035',
                           'header_sha1_verified': True, 'header_adler32_verified': True},
              'tables': {'string_ids': dex.string_count, 'type_ids': dex.type_count,
                         'class_defs': dex.class_count, 'package_count': len(packages)},
              'official_unity_jar': {'path': str(jar), 'sha256': hashlib.sha256(jar.read_bytes()).hexdigest(),
                                     'class_names': len(official_names)},
              'classes_by_category': dict(totals), 'packages': packages,
              'scope_decision': 'PENDING USER; no Java code added to reconstruction scope',
              'limitations': ['No class_data, code_item, methods, instructions, decompilation or target execution.',
                              'SDK provenance and vendor ownership are inferred unless official class inventory corroborates them.',
                              'Kairo namespace may contain shared framework, bridges or third-party-derived code; game logic content UNKNOWN.',
                              'Generated R resource classes are application-specific names but are not evidence of handwritten game logic.']}
    output = ROOT / 'reports/DEX-package-inventory.json'
    output.write_text(json.dumps(report, indent=2, ensure_ascii=True) + '\n', encoding='utf-8')
    print(json.dumps({key: report[key] for key in ('artifact', 'tables', 'classes_by_category', 'scope_decision')}))


if __name__ == '__main__':
    main()
