"""Policy boundary checks: an incomplete model never vetoes compilation."""
import json
from round8_model_policy import prediction_allows_compilation,compiled_signature_allows_search,reserve_incomplete_variant
def main():
    desired=['System.Int32'];wrong={'complete':True,'predicted_types':['System.Boolean']}
    partial=dict(wrong,complete=False)
    assert not prediction_allows_compilation(wrong,desired)
    assert prediction_allows_compilation(partial,desired)
    assert compiled_signature_allows_search(partial,desired,['exact'],['exact'])[0]
    assert not compiled_signature_allows_search(wrong,desired,['exact'],['exact'])[0]
    assert not compiled_signature_allows_search(partial,desired,['different'],['exact'])[0]
    row={}
    assert all(reserve_incomplete_variant(row,12) for _ in range(12))
    assert not reserve_incomplete_variant(row,12)
    print(json.dumps({'policy_checks':6,'passed':True,'row':row}))
if __name__=='__main__':main()
