"""Bind a sequential trial to its real full and filtered C# inputs."""
import hashlib
from pathlib import Path
def write_candidate_sources(candidate,full_path,screen_paths=None,logical=None,source_filter=None):
    full=Path(full_path);full.write_text(candidate,encoding='utf-8',newline='')
    proof={'full_source_path':str(full),'full_source_sha256':hashlib.sha256(full.read_bytes()).hexdigest()}
    if screen_paths and source_filter:
        screen=Path(screen_paths[logical])
        if screen.resolve()==full.resolve():raise ValueError('A filtered candidate needs a separate source file')
        text=source_filter(candidate);screen.write_text(text,encoding='utf-8',newline='')
        expected=hashlib.sha256(text.encode('utf-8')).hexdigest()
        if hashlib.sha256(screen.read_bytes()).hexdigest()!=expected:raise ValueError('Candidate screen write mismatch')
        proof.update(screen_source_path=str(screen),screen_source_sha256=expected,screen_updated_from_candidate=True)
    return proof
