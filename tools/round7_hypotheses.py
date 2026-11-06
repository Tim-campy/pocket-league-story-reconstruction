"""Derive hypothesis outcomes from independent fixtures and real engine results."""
import json
from pathlib import Path

ROOT=Path(__file__).resolve().parents[1]


def main():
    docs=[]
    for p in (ROOT/'reports').glob('round7-*-engine.json'):
        d=json.loads(p.read_text(encoding='utf-8-sig'))
        if d.get('completed'):docs.append(d)
    methods={m['id']:m for d in docs for m in d['methods'] if m['body_equal']}
    tests=[
        ('Local para valor testado de qualquer tipo','data.LeagueData::IsVictoryLeague()->System.Boolean'),
        ('Resultado local com retorno unico','data.PlayerData::GetPhyParamBarW(System.Int32,System.Int32)->System.Int32'),
        ('Switch no parametro e saida comum void','surface.GameView::OnTouchDlgSel(System.Int32,System.Int32,kairo.unity.surface.TouchEvent)->System.Void'),
        ('Objeto em local antes das atribuicoes e de Push','system.form.FriendForm::<_update>m__17()->System.Void'),
        ('Foreach sobre array com copia e indice sinteticos','kairo.unity.surface.SurfaceManagerBase::GetPaintRect(UnityEngine.Vector2[])->System.Int32[]'),
        ('Struct sem inicializador antes das atribuicoes de campos','kairo.unity.ui.Graphics::ArgbToColor(System.Int32)->UnityEngine.Color'),
        ('Escopo do indice explica slots em Canvas','kairo.unity.ui.Canvas::EndAlterStaticRender(kairo.unity.ui.Graphics,System.Int32)->System.Void'),
        ('Escopo do indice explica slots em MeshBuilder','kairo.unity.graphics.MeshBuilder::AddQuad(UnityEngine.Vector3[],System.Single,System.Single,System.Single,System.Single,UnityEngine.Color)->System.Void'),
        ('Array do foreach icado em <>7__wrap1, sem copia explicita redundante','kairo.unity.util.FastVector`1/<GetEnumerator>d__42::MoveNext()->System.Boolean')]
    hypotheses=[]
    for text,identity in tests:
        m=methods.get(identity)
        hypotheses.append({'status':'CONFIRMADA' if m else 'INCONCLUSIVA','hypothesis':text,
            'evidence':identity+(': corpo integral igual; regras '+', '.join(s['rule'] for s in m['steps']) if m else ': nenhum corpo integral aprovado registrado')})
    hypotheses.extend([
        {'status':'REFUTADA','hypothesis':'A cadeia de zeros isolada basta para SubForm._draw2','evidence':'Modelo passou de 59 para 56 slots, mas original tem 55; dois switches recompilados compartilham um temporario Int32 ausente no original.'},
        {'status':'REFUTADA','hypothesis':'Cadeia de quatro zeros mais switches convertidos em if basta para resolver SubForm._draw2 inteiro','evidence':'Na Parte A a assinatura de 55 slots ficou exata e o prefixo bruto avancou de 2 a 2218, mas o corpo nao fechou. Os outros desvios exigiram transformacoes adicionais; reports/round7-smcs-zerochain-diagnosis.json.'},
        {'status':'REFUTADA','hypothesis':'Todo elemento foreach cujo primeiro uso ocorre com pilha ocupada precisa de slot Roslyn','evidence':'Ampliacao produziu oito previsoes completas incorretas nos matched; retirada. A regra adotada exige prova mais estreita por uso do endereco da struct.'},
        {'status':'CONFIRMADA','hypothesis':'O modelo smcs recupera os 43 casos mapeados incompletos anteriores','evidence':'43/43 completos e exatos, incluindo sete storeys; validacao integral 1617/1617 previsoes completas corretas nos 1697 corpos matched.'},
        {'status':'REFUTADA','hypothesis':'Ordem textual dos tipos basta para numerar todos os metodos anonimos smcs','evidence':'BillingManager era previsto como m__E em vez de m__10; codigo class.cs e RootContext confirmam emissao topologica de classes base antes das derivadas.'}])
    hypotheses.append({'status':'REFUTADA','hypothesis':'Ultimo comando de um bloco lexical implica fim do metodo e autoriza tail-early-return',
        'evidence':'Em SubForm._draw2 o br original 9268 vai a 9358 (incremento do slot47); em Player.Act_dribble o br638 vai a776 (incremento do slot6). A regra antiga inseriu ret em corpos ainda divergentes; nenhum matched foi perdido. A nova regra exige fim comprovado do metodo; retorno em laco e diagnosticado separadamente.'})
    journal_path=ROOT/'reports/round7-cycle-journal.json'
    if journal_path.exists():
        journal=json.loads(journal_path.read_text(encoding='utf-8'))
        for row in journal['cycles']:
            hypotheses.append({'status':'CONFIRMADA' if row['methods_gained'] else 'INCONCLUSIVA',
                'hypothesis':row['rule']+': '+row['hypothesis'],
                'evidence':row['label']+': '+str(row['methods_gained'])+' novos matched, '+str(row['il_bytes_gained'])+' bytes; '+row['result']})
    coalesce=ROOT/'reports/round7-coalesce-patches.json'
    if coalesce.exists():
        hypotheses.append({'status':'REFUTADA','hypothesis':'Uma assinatura com tipos iguais basta para posicionar os slots Int32 sem uso',
            'evidence':'Em UserData.Draw_edit2 o primeiro candidato posicionava as reservas em 10/40, apesar dos mesmos tipos; foi rejeitado. O modelo da proposta final confirmou exatamente os slots 8/38 e o verificador fechou o corpo de 6626 bytes.'})
    output=ROOT/'reports/round7-hypotheses.json'
    output.write_text(json.dumps({'hypotheses':hypotheses},indent=2,ensure_ascii=False)+'\n',encoding='utf-8',newline='\n')


if __name__=='__main__':main()
