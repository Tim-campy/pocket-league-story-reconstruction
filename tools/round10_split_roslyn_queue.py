"""Give each prepared Roslyn diagnostic family its own measured cycle."""
import json
from pathlib import Path
from compare_kairo import sha

ROOT = Path(__file__).resolve().parents[1]


def main():
    path = ROOT / 'config/round10-protocol.json'
    cfg = json.loads(path.read_text(encoding='utf-8'))
    parent = next(x for x in cfg['prepared_queue'] if x['id'] == 'd-ready-roslyn-lifetimes')
    assert parent['status'] == 'pending'
    bundle_path = ROOT / parent['report']
    assert sha(bundle_path) == parent['report_sha256']
    bundle = json.loads(bundle_path.read_text(encoding='utf-8'))
    children = []
    for name, patch in zip(('material-scope', 'language-body-local', 'http-dispatch'), bundle['patches'], strict=True):
        item_id = 'd-ready-roslyn-' + name
        assert not any(x['id'] == item_id for x in cfg['prepared_queue'])
        child = dict(parent)
        child.update(id=item_id, method_ids=[patch['method_id']], rule=patch['rule'],
                     parent_queue_id=parent['id'], status='pending')
        cfg['prepared_queue'].append(child)
        children.append(item_id)
    parent.update(status='split_into_pending_children', children=children)
    cfg.setdefault('preparation_notes', []).append(
        'The three prepared Roslyn lifetime/dispatch compositions have distinct aligned pairs; '
        'their queue item was split into three pending diagnostic families, without assuming any target gain.')
    path.write_text(json.dumps(cfg, indent=2) + '\n', encoding='utf-8')
    print(json.dumps({'queue_split': children, 'method_states_modified': False}))


if __name__ == '__main__':
    main()
