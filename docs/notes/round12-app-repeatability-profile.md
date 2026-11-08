# App repeatability proof compatibility

The first App raw measurement at the B33 threshold stopped before comparing bytes: its strict profile reader required an aggregate field absent from the older Round3 JSON schema. B33's verified gain of one method and 253 IL bytes was already journaled and was not repeated.

The [derived profile](../../reports/round12-Assembly-CSharp-repeatability-profile.json) revalidates both retained Round3 PEs physically against their original hashes: both 893440 bytes, exactly 17 unequal offsets within the 16-byte MVID and 4-byte COFF timestamp fields, and zero unexplained offsets. It also binds the 97 retained sources, compiler/runtime, flags, references and manifests. The [review](../../reports/round12-app-profile-compatibility-review.json) records this compatibility correction. Original proof and raw comparator are unchanged.

This step ran zero compilers, changed no DLL or method state, and added no exception field. Toolchain configuration points its two App evidence entries to the new derived proof. A fresh commit-only verification renews snapshot and raw provenance without recounting B33.
