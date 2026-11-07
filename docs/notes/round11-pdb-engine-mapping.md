# Native PDB mapping in the private candidate engine

The first C1 pass, journal c01, compiled no hypothesis: all 24 pending Kairo
methods stopped at source_mapping_UNKNOWN. The historical /pdb: path was passed
to csc correctly, but the direct private compiler did not mirror that PDB next
to its private DLL. The independent symbol reader therefore reported NONE.
This is an infrastructure result, not a refuted source hypothesis. The original
journal and zero gains remain intact; C1 is partial until a new effective pass.

The new round11_pdb_mirror helper reads the PE's RSDS record and native MSF7
information stream. GUID and age must match before the compiler-produced PDB is
copied next to the private DLL. The PE SHA is checked before and after; no PE,
IL, metadata or PDB byte is rewritten. Only an analysis sidecar is copied.

One independently authored fixture compiled with pinned csc 1.1.0 redirected
its PDB to another owned directory. Before mirroring: NONE and no points. After:
PDB with three sequence points; every complete method body and the DLL SHA were
unchanged. Two negative fixture inputs changed only the owned PDB's GUID or age;
both were rejected before copying, and the valid input was restored exactly.
See reports/round11-pdb-mirror-fixture.json. No game target was compiled by this
fixture test. The actual C1 candidate remains UNKNOWN until the new oracle pass.
