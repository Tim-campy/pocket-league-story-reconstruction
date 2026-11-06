"""Rebasing hypotheses must preserve surrounding code and require identity."""
import unittest
from round6_solver_alternatives import rebase_variant,priority


class BaselineAlternativeTests(unittest.TestCase):
    def test_current_method_is_replaced_without_overwriting_neighbor(self):
        source='// \U0001f600\nvoid M(){int x; x=0;}\nvoid N(){Keep();}'
        start=len(source[:source.index('void M')].encode('utf-16-le'))//2
        end=len(source[:source.index('\nvoid N')].encode('utf-16-le'))//2
        variant={'method_id':'T::M()->System.Void','rule':'solver-baseline-group-original-zero-chain',
                 'after':'void M(){int x,y; x=y=0;}'}
        patch=rebase_variant(source,start,end,variant,method_id=variant['method_id'])
        raw=source.encode('utf-16-le')
        actual=(raw[:patch['start']*2]+patch['after'].encode('utf-16-le')+raw[patch['end']*2:]).decode('utf-16-le')
        self.assertEqual(actual,'// \U0001f600\nvoid M(){int x,y; x=y=0;}\nvoid N(){Keep();}')
        self.assertEqual(patch['before'],'void M(){int x; x=0;}')
        self.assertTrue(patch['requires_fresh_model_prediction'])
        self.assertTrue(patch['requires_strict_prefix_and_full_verification'])

    def test_different_method_id_rejected(self):
        with self.assertRaises(ValueError):
            rebase_variant('void M(){}',0,10,{'method_id':'T::N()','after':'void N(){}','rule':'solver'},method_id='T::M()')

    def test_surrogate_boundary_rejected(self):
        with self.assertRaises(ValueError):
            rebase_variant('\U0001f600void M(){}',1,12,{'method_id':'M','after':'void M(){}','rule':'solver'},method_id='M')

    def test_atomic_zero_alternative_has_priority(self):
        self.assertLess(priority({'rule':'solver-baseline-group-original-zero-chain'}),priority({'rule':'solver-baseline-group-block-declarations'}))
        self.assertLess(priority({'rule':'solver-baseline-group-all-calculated-blocks'}),priority({'rule':'solver-baseline-group-block-declarations'}))


if __name__=='__main__':unittest.main()
