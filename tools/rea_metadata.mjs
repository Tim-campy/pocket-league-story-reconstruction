// Supplement retained MCP evidence with nested/type-reference tables and full
// instruction listings, using the already installed REA parser. No target code
// is loaded or executed. These internal APIs are pinned to REA 6.5.0.
import fs from 'node:fs';
import path from 'node:path';
import {pathToFileURL} from 'node:url';
import {createHash} from 'node:crypto';

const [reaRoot, input, output, bundlePath] = process.argv.slice(2);
if (!bundlePath) throw new Error('Usage: node tools/rea_metadata.mjs REA_ROOT DLL OUTPUT_JSON REA_BUNDLE');
const pkg = JSON.parse(fs.readFileSync(path.join(reaRoot, 'package.json'), 'utf8'));
if (pkg.version !== '6.5.0') throw new Error('REA parser version changed; review adapter before using it');
const load = name => import(pathToFileURL(path.join(reaRoot, 'dist/dotnet', name + '.js')).href);
const {readManagedPeLayout} = await load('ManagedPeReader');
const {readManagedMetadataLayout} = await load('ManagedMetadataLayout');
const {metadataRowCursor, readMetadataString, readMetadataBlob} = await load('ManagedMetadataHeaps');
const {specSignature} = await load('ManagedMemberInspectorCore');
const {decodeInstructions} = await load('ManagedMemberInstructionDecoder');
const bytes = fs.readFileSync(input);
const sha256 = data => createHash('sha256').update(data).digest('hex');
const hash = sha256(bytes);
const evidence = JSON.parse(fs.readFileSync(bundlePath, 'utf8')).records.find(
  r => r.operation === 'inspect_managed_members' && r.normalized_result.artifact.sha256 === hash);
if (!evidence) throw new Error('No retained method Evidence for this exact assembly');
const pe = readManagedPeLayout(bytes);
if (!pe.cli) throw new Error('No CLI metadata');
const layout = readManagedMetadataLayout(bytes,
  pe.rvaToOffset(pe.cli.metadata.rva, pe.cli.metadata.size, 'cli.metadata'), pe.cli.metadata.size);
const str = index => readMetadataString(bytes, layout, index, layout.strings.size);
const blob = index => readMetadataBlob(bytes, layout, index, layout.blob.size);
const rows = (table, read) => Array.from({length:layout.table(table)?.rowCount ?? 0}, (_, i) =>
  ({row:i + 1, ...read(metadataRowCursor(bytes, layout, table, i + 1))}));
const typeRefs = rows(1, c => ({scope:c.readIndex(layout.codedIndexSize('ResolutionScope')),
  name:str(c.readIndex(layout.stringIndexSize)), namespace:str(c.readIndex(layout.stringIndexSize))}));
const nested = rows(41, c => ({nested:c.readIndex(layout.tableIndexSize(2)), parent:c.readIndex(layout.tableIndexSize(2))}));
const typeSpecs = rows(27, c => {
  const b = blob(c.readIndex(layout.blobIndexSize));
  return {raw_hex:b.toString('hex'), decoded:specSignature(layout, b, {form:'type',scope:'metadata.TypeSpec',offset:c.start})[0]};
});
const standalone = rows(17, c => ({raw_hex:blob(c.readIndex(layout.blobIndexSize)).toString('hex')}));
const methods = evidence.normalized_result.methods.map(method => {
  const body = method.body;
  if (body.status === 'absent') return {token:method.token, instructions:[], issue:null};
  if (body.status !== 'present') throw new Error('Incomplete original method: ' + method.token);
  const offset = body.file_offset;
  const header = (bytes[offset] & 3) === 2 ? 1 : (bytes.readUInt16LE(offset) >>> 12) * 4;
  const il = bytes.subarray(offset + header, offset + header + body.il_size);
  if (sha256(il) !== body.il_sha256) throw new Error('CIL digest mismatch');
  const decoded = decodeInstructions(il);
  if (decoded.issue || decoded.truncated || decoded.count !== body.instruction_count)
    throw new Error('Incomplete instruction decode: ' + method.token);
  return {token:method.token, instructions:decoded.parsed, issue:decoded.issue};
});
const identities = JSON.parse(fs.readFileSync(bundlePath,'utf8')).records.find(
  r => r.operation === 'inspect_managed_artifact' && r.normalized_result.artifact.sha256 === hash)?.normalized_result;
const fingerprints = Object.fromEntries(['ManagedPeReader','ManagedMetadataLayout','ManagedMetadataHeaps',
  'ManagedMemberInspectorCore','ManagedMemberInstructionDecoder'].map(name =>
  [name + '.js', sha256(fs.readFileSync(path.join(reaRoot,'dist/dotnet',name + '.js')))]));
fs.mkdirSync(path.dirname(output),{recursive:true});
fs.writeFileSync(output, JSON.stringify({provider:'rea-agents',version:pkg.version,parser_fingerprints:fingerprints,
  artifact_sha256:hash,evidence_id:evidence.evidence_id,type_refs:typeRefs,nested_classes:nested,type_specs:typeSpecs,
  standalone_signatures:standalone,assembly_refs:identities?.references ?? [],
  attributes:identities?.attributes ?? [],methods}, null, 2) + '\n');
console.log(JSON.stringify({input,output,methods:methods.length,nested_classes:nested.length,type_refs:typeRefs.length}));
