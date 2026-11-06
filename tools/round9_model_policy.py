"""Model predictions guide source candidates; compiled bodies remain the oracle."""
def prediction_allows_compilation(prediction,expected_types):
    return prediction.get('complete') is not True or prediction.get('predicted_types')==expected_types

def compiled_signature_allows_search(prediction,actual_types,actual_signature,original_signature):
    if prediction.get('complete') is True and actual_types!=prediction.get('predicted_types'):
        return False,'complete_prediction_disagrees_with_emitter'
    if actual_signature!=original_signature:
        return False,'compiled_signature_differs_from_original'
    return True,'compiled_signature_verified'

def reserve_incomplete_variant(row,limit):
    used=row.setdefault('incomplete_variants_compiled',0)
    row['incomplete_variant_limit']=limit
    if used>=limit:
        row['incomplete_budget_skipped']=row.get('incomplete_budget_skipped',0)+1
        return False
    row['incomplete_variants_compiled']=used+1
    return True
