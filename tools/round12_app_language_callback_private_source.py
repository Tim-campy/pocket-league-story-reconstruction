"""Build and initialize the new form before loading its Push receiver."""
import re
RULE='r12-smcs-language-callback-explicit-object-local-before-push-receiver'
METHOD_ID='system.form.LanguageForm::<Update>m__20(system.form.HttpForm,system.form.HttpForm/ResultData)->System.Void'
def transform(method):
 pattern=re.compile(r'instance\.Push\(new LanguageForm\s*\{\s*mode_ = 1\s*\}\);')
 out,count=pattern.subn('LanguageForm nextLanguageForm = new LanguageForm();\nnextLanguageForm.mode_ = 1;\ninstance.Push(nextLanguageForm);',method)
 if count!=1:raise ValueError('Unique language form object initializer required')
 return out
