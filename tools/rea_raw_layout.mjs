// Read-only PE/CLI coordinates for raw measurement. No target CLR load.
// REA 6.5.0 supplies all section geometry and ECMA table/heap layouts.
import fs from 'node:fs';
import path from 'node:path';
import {pathToFileURL} from 'node:url';
import {createHash} from 'node:crypto';

const [reaRoot, target] = process.argv.slice(2);
if (!target) throw new Error('Usage: node tools/rea_raw_layout.mjs REA_ROOT DLL');
const pkg = JSON.parse(fs.readFileSync(path.join(reaRoot,'package.json'),'utf8'));
if (pkg.version !== '6.5.0') throw new Error('REA parser version changed; review adapter');
const load = name => import(pathToFileURL(path.join(reaRoot,'dist/dotnet',name+'.js')).href);
const {readManagedPeLayout} = await load('ManagedPeReader');
const {readManagedMetadataLayout} = await load('ManagedMetadataLayout');
const bytes = fs.readFileSync(target);
const sha = data => createHash('sha256').update(data).digest('hex');
const range = (offset,size,scope) => {
  if (!Number.isSafeInteger(offset)||!Number.isSafeInteger(size)||offset<0||size<0||offset>bytes.length-size)
    throw new Error('Out-of-file extent: '+scope);
};
const pe = readManagedPeLayout(bytes);
if (!pe.cli || pe.cliIssue) throw new Error('No complete PE/CLI layout');
const metadataOffset = pe.rvaToOffset(pe.cli.metadata.rva,pe.cli.metadata.size,'raw.metadata');
const layout = readManagedMetadataLayout(bytes,metadataOffset,pe.cli.metadata.size);
const peOffset=bytes.readUInt32LE(60), optionalOffset=peOffset+24;
const optionalSize=bytes.readUInt16LE(peOffset+20);
const sectionTable=optionalOffset+optionalSize;
range(optionalOffset,64,'optional header');
const headersSize=bytes.readUInt32LE(optionalOffset+60);
range(0,headersSize,'PE SizeOfHeaders');
range(sectionTable,pe.sectionCount*40,'section names');
if(headersSize<sectionTable+pe.sectionCount*40)
  throw new Error('PE SizeOfHeaders does not cover the section headers');
const sections=pe.sections.map((s,index)=>{
  const at=sectionTable+index*40;
  const name=bytes.subarray(at,at+8).toString('ascii').replace(/\0.*$/s,'');
  range(s.rawOffset,s.rawSize,'section '+name);
  if (bytes.readUInt32LE(at+16)!==s.rawSize||bytes.readUInt32LE(at+20)!==s.rawOffset)
    throw new Error('REA section geometry disagrees with admitted header');
  return {name,ordinal:index+1,offset:s.rawOffset,size:s.rawSize,
    virtual_address:s.virtualAddress,virtual_size:s.virtualSize};
});

// REA returns the admitted stream names and the four heap descriptors. Recover
// every raw header coordinate, then corroborate those admitted descriptors.
const versionLength=bytes.readUInt32LE(metadataOffset+12);
let cursor=metadataOffset+Math.ceil((16+versionLength)/4)*4;
range(cursor,4,'metadata flags/count');
const count=bytes.readUInt16LE(cursor+2);cursor+=4;
const streams=[];
for(let index=0;index<count;index++){
  range(cursor,8,'stream header');
  const offset=metadataOffset+bytes.readUInt32LE(cursor),size=bytes.readUInt32LE(cursor+4);
  let end=cursor+8;
  while(end<Math.min(cursor+40,bytes.length)&&bytes[end]!==0)end++;
  if(end===Math.min(cursor+40,bytes.length))throw new Error('Missing bounded stream name');
  const name=bytes.subarray(cursor+8,end).toString('ascii');
  range(offset,size,'stream '+name);
  if(offset<metadataOffset||offset+size>metadataOffset+pe.cli.metadata.size)
    throw new Error('Stream leaves metadata root');
  streams.push({name,offset,size,header_offset:cursor});
  cursor=Math.ceil((end+1)/4)*4;
}
if(JSON.stringify(streams.map(s=>s.name).sort())!==JSON.stringify(layout.streamNames))
  throw new Error('Stream headers disagree with admitted REA inventory');
for(const descriptor of [layout.strings,layout.blob,layout.guid,layout.userStrings].filter(Boolean)){
  const found=streams.find(s=>s.name===descriptor.name);
  if(!found||found.offset!==descriptor.offset||found.size!==descriptor.size)
    throw new Error('Heap coordinates disagree with REA');
}
const tablesStream=streams.find(s=>s.name==='#~')??streams.find(s=>s.name==='#-');
if(!tablesStream)throw new Error('No tables stream');
const tables=[...layout.tables.values()].map(t=>{
  const size=t.rowCount*t.rowSize;
  range(t.offset,size,'table '+t.name);
  if(t.offset<tablesStream.offset||t.offset+size>tablesStream.offset+tablesStream.size)
    throw new Error('Table leaves admitted table stream');
  return {name:t.name,index:t.index,offset:t.offset,size,row_count:t.rowCount,row_size:t.rowSize};
});
const parserFiles=['dotnet/ManagedPeReader.js','dotnet/ManagedMetadataLayout.js',
  'dotnet/ManagedMetadataRowSizes.js','dotnet/ManagedReaderFailure.js',
  'dotnet/ManagedDecodeBudget.js','domain/peMachine.js'];
const parserHashes=Object.fromEntries(parserFiles.map(file=>[file,sha(fs.readFileSync(path.join(reaRoot,'dist',file)))]));
console.log(JSON.stringify({schema_version:1,provider:'rea-agents',version:pkg.version,
  parser_hashes:parserHashes,artifact:{path:path.resolve(target),sha256:sha(bytes),size:bytes.length},
  headers:{name:'PE_HEADERS',offset:0,size:headersSize},sections,
  metadata:{offset:metadataOffset,size:pe.cli.metadata.size,version:layout.version,
    rva:pe.cli.metadata.rva,stream_header_end:cursor},streams,tables,
  coverage:{state:'complete-for-admitted-layout',target_executed:false,token_resolution:false,
    normalized_fields:[],supported_tables:'ECMA-335 tables admitted by REA 6.5.0'},
  index_widths:{strings:layout.stringIndexSize,guid:layout.guidIndexSize,blob:layout.blobIndexSize}}));
