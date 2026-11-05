// Resolve Module.Mvid through Module row 1 and #GUID, never by byte search.
// Read-only adapter pinned to the already installed REA 6.5.0 metadata parser.
import fs from 'node:fs';
import path from 'node:path';
import {pathToFileURL} from 'node:url';
import {createHash} from 'node:crypto';

const [reaRoot, target] = process.argv.slice(2);
if (!target) throw new Error('Usage: node tools/rea_mvid_coordinate.mjs REA_ROOT DLL');
const pkg = JSON.parse(fs.readFileSync(path.join(reaRoot, 'package.json'), 'utf8'));
if (pkg.version !== '6.5.0') throw new Error('REA parser version changed; review adapter');
const load = name => import(pathToFileURL(path.join(reaRoot, 'dist/dotnet', name + '.js')).href);
const {readManagedPeLayout} = await load('ManagedPeReader');
const {readManagedMetadataLayout} = await load('ManagedMetadataLayout');
const {metadataRowCursor} = await load('ManagedMetadataHeaps');
const bytes = fs.readFileSync(target);
const pe = readManagedPeLayout(bytes);
if (!pe.cli) throw new Error('No PE/CLI metadata');
const layout = readManagedMetadataLayout(bytes,
  pe.rvaToOffset(pe.cli.metadata.rva, pe.cli.metadata.size, 'cli.metadata'), pe.cli.metadata.size);
if (layout.table(0)?.rowCount !== 1) throw new Error('Expected one Module row');
const row = metadataRowCursor(bytes, layout, 0, 1);
const generation = row.readUInt16();
const nameIndex = row.readIndex(layout.stringIndexSize);
const mvidIndex = row.readIndex(layout.guidIndexSize);
if (mvidIndex < 1 || mvidIndex * 16 > layout.guid.size) throw new Error('MVID outside #GUID');
const mvidOffset = layout.guid.offset + (mvidIndex - 1) * 16;
const peOffset = bytes.readUInt32LE(60);
if (bytes.readUInt32LE(peOffset) !== 0x00004550) throw new Error('Invalid PE signature');
const sha = data => createHash('sha256').update(data).digest('hex');
const parserHashes = Object.fromEntries(['ManagedPeReader', 'ManagedMetadataLayout', 'ManagedMetadataHeaps'].map(
  name => [name + '.js', sha(fs.readFileSync(path.join(reaRoot, 'dist/dotnet', name + '.js')))]));
console.log(JSON.stringify({provider: 'rea-agents', version: pkg.version, parser_hashes: parserHashes,
  artifact_sha256: sha(bytes), module_row_offset: row.start, module_generation: generation,
  module_name_index: nameIndex, mvid_index: mvidIndex, guid_heap_offset: layout.guid.offset,
  mvid_offset: mvidOffset, mvid_hex_le: bytes.subarray(mvidOffset, mvidOffset + 16).toString('hex'),
  timestamp_offset: peOffset + 8, timestamp: bytes.readUInt32LE(peOffset + 8)}));
