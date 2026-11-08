"""Return the existing result local after the protected region."""
RULE='r12-smcs-maintenance-result-local-after-try-catch'
METHOD_ID='system.billing.ui.StoreBase::CheckPaymentMaintenance()->System.Boolean'
def transform(method):
 if method.count('bool flag = true;')!=1 or method.count('return false;')!=1 or method.count('throw ex;')!=1:raise ValueError('Exact existing result/try/catch required')
 text=method.replace('return false;','flag = false;',1);end=text.rfind('}')
 if end<0:raise ValueError('Missing method end')
 return text[:end]+'return flag;\n'+text[end:]
