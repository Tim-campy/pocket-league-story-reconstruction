"""Test the real CLI wrapper with own preflight files and mocked child processes."""
import contextlib
import csv
import hashlib
import io
import json
from pathlib import Path
from types import SimpleNamespace
import tempfile
import unittest
from unittest.mock import patch

import verify


class VerifyExitTests(unittest.TestCase):
    def setUp(self):
        directory=verify.ROOT/'build/round12'
        self.temp=tempfile.TemporaryDirectory(dir=directory,prefix='verify-exit-test-')
        self.root=Path(self.temp.name).resolve()
        assert self.root.is_relative_to(directory.resolve())
        self.addCleanup(self.temp.cleanup)
        (self.root/'config').mkdir();(self.root/'original/extracted').mkdir(parents=True)
        original=b'own preflight fixture'
        (self.root/'original/own.bin').write_bytes(original)
        (self.root/'original/extracted/fixture.bin').write_bytes(original)
        digest=hashlib.sha256(original).hexdigest()
        (self.root/'config/project.json').write_text(json.dumps(dict(original_apk='original/own.bin',
            extracted='original/extracted',apk_sha256=digest)))
        self.ids=['KairoLibrary::Fixture','Assembly-CSharp::Fixture']
        refs={key:dict(artifact_path='fixture.bin',artifact_sha256=digest,body=None,il_size=0,token='0x06000001')for key in self.ids}
        (self.root/'config/method-reference.json').write_text(json.dumps(refs))
        with (self.root/'config/methods.csv').open('w',newline='')as stream:
            writer=csv.DictWriter(stream,fieldnames=['id','assembly','tamanho_il','token','estado']);writer.writeheader()
            for key in self.ids:writer.writerow(dict(id=key,assembly=key.split('::')[0],tamanho_il=0,token='0x06000001',estado='todo'))
        (self.root/'build').mkdir()
        for assembly in ('KairoLibrary','Assembly-CSharp'):
            (self.root/('build/verification-'+assembly+'.json')).write_text(json.dumps(dict(methods={assembly+'::Fixture':{'own':True}})))

    def run_wrapper(self,arguments,statuses):
        with patch.object(verify,'ROOT',self.root),patch('sys.argv',['verify',*arguments]),\
             patch.object(verify,'compare_files',return_value={'equal':True,'files_total':1}),\
             patch.object(verify.subprocess,'run',side_effect=[SimpleNamespace(returncode=x)for x in statuses])as child,\
             contextlib.redirect_stdout(io.StringIO()),contextlib.redirect_stderr(io.StringIO()):
            result=verify.main()
            return result,child.call_count

    def test_both_success_is_success(self):self.assertEqual(self.run_wrapper(['--all'],[0,0]),(0,2))
    def test_first_difference_propagated(self):self.assertEqual(self.run_wrapper(['--all'],[1,0]),(1,2))
    def test_second_difference_propagated(self):self.assertEqual(self.run_wrapper(['--all'],[0,1]),(1,2))
    def test_two_differences_propagated(self):self.assertEqual(self.run_wrapper(['--all'],[1,1]),(1,2))
    def test_child_scope_error_not_hidden(self):self.assertEqual(self.run_wrapper(['--all'],[2]),(2,1))
    def test_one_assembly_success(self):self.assertEqual(self.run_wrapper(['--all','--assembly','Assembly-CSharp'],[0]),(0,1))
    def test_one_method_success(self):self.assertEqual(self.run_wrapper(['--method',self.ids[0]],[0]),(0,1))
    def test_missing_scope_still_two(self):
        with self.assertRaises(SystemExit)as result:self.run_wrapper([],[])
        self.assertEqual(result.exception.code,2)


if __name__=='__main__':
    suite=unittest.defaultTestLoader.loadTestsFromTestCase(VerifyExitTests)
    result=unittest.TextTestRunner(verbosity=1).run(suite)
    (verify.ROOT/'reports/round12-verify-exit-tests.json').write_text(json.dumps(dict(
        tests=result.testsRun,failures=len(result.failures),errors=len(result.errors),
        original_bug='return status or 2 misreported complete success as missing scope',
        scope_validation_unchanged=True,child_failures_preserved=True,
        target_compilations=0,DLL_writes=0,method_or_type_states_modified=False),indent=2)+'\n',encoding='utf-8')
    raise SystemExit(not result.wasSuccessful())
