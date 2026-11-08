"""Pure measured guard compositions, preserving loop transfer owners."""
import re
IDS={'tenant':'data.TenantData::StrengthTenant()->System.Void',
     'billing':'system.billing.ui.BillingManager::AddPurchaseLog(System.String,System.Int32,system.billing.util.Purchase)->System.Void'}
RULES={'tenant':'r12-strength-first-continue-and-positive-id-body',
       'billing':'r12-purchaselog-first-continue-and-positive-order-body'}
TENANT='if ((tenant.state_ == 0) || (tenant.tenantDataId_ != id_)) { continue; }'
BILLING='if ((!(purchaseRecord2.productId_.Equals(productId))) || ((text != null) && (!(purchaseRecord2.orderId_.Equals(text))))) { continue; }'

def closing(source,start):
    depth=0
    for i in range(start,len(source)):
        if source[i]=='{':depth+=1
        elif source[i]=='}':
            depth-=1
            if depth==0:return i
    raise ValueError('unbalanced braces')

def transform_strength(source,form='combined'):
    pattern=re.escape(TENANT)+r'\s*tenant\.StrenghTenant\(\);'
    if len(re.findall(pattern,source))!=1:raise ValueError('unique measured tenant guard/tail required')
    if source.count('tenant.StrenghTenant();')!=1:raise ValueError('duplicate or unmeasured tenant tail')
    if source.count('for (int i = 0; i < udat_.tenants_.Length; i++)')!=1:raise ValueError('measured tenant loop required')
    if form=='combined':new='if (tenant.state_ == 0) { continue; }\r\nif (tenant.tenantDataId_ == id_) { tenant.StrenghTenant(); }'
    elif form=='split-only':new='if (tenant.state_ == 0) { continue; }\r\nif (tenant.tenantDataId_ != id_) { continue; }\r\ntenant.StrenghTenant();'
    elif form=='positive-only':new='if ((tenant.state_ != 0) && (tenant.tenantDataId_ == id_)) { tenant.StrenghTenant(); }'
    else:raise ValueError('unknown fixture form')
    return re.sub(pattern,lambda _:new,source)

def transform_purchase_log(source,form='combined'):
    pattern=re.escape(BILLING)+r'\s*purchaseRecord = purchaseRecord2;\s*break;'
    if len(re.findall(pattern,source))!=1:raise ValueError('unique measured billing guard/tail required')
    if source.count('for (int num = purchaseLog_.Size() - 1; num >= 0; num--)')!=1:raise ValueError('measured descending loop required')
    first='if (!purchaseRecord2.productId_.Equals(productId)) { continue; }'
    positive='(text == null) || purchaseRecord2.orderId_.Equals(text)'
    tail='purchaseRecord = purchaseRecord2; break;'
    if form=='combined':new=first+'\r\nif ('+positive+') { '+tail+' }'
    elif form=='split-only':new=first+'\r\nif ((text != null) && !purchaseRecord2.orderId_.Equals(text)) { continue; }\r\n'+tail
    elif form=='positive-only':new='if (purchaseRecord2.productId_.Equals(productId) && ('+positive+')) { '+tail+' }'
    else:raise ValueError('unknown fixture form')
    return re.sub(pattern,lambda _:new,source)
