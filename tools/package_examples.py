"""Create deterministic, ready-to-extract tutorial archives and checksums."""
from pathlib import Path
import hashlib
from zipfile import ZipFile, ZipInfo, ZIP_STORED

ROOT = Path(__file__).resolve().parents[1]
MODS = ROOT/'example/mods'
OUT = ROOT/'downloads'

def archive(path, folders):
    # Stored entries avoid platform/zlib-version differences in committed archives.
    with ZipFile(path, 'w', compression=ZIP_STORED) as z:
        for folder in folders:
            for file in sorted(folder.rglob('*')):
                if file.is_file():
                    name = file.relative_to(MODS).as_posix()
                    info = ZipInfo(name, (2026,9,24,0,0,0))
                    info.compress_type = ZIP_STORED
                    info.create_system = 3
                    info.external_attr = 0o100644 << 16
                    z.writestr(info, file.read_bytes())
    with ZipFile(path) as z:
        assert z.testzip() is None
        for folder in folders:
            assert folder.name+'/modinfo.json' in z.namelist()
            assert folder.name+'/img.png' in z.namelist()
        for entry in z.infolist():
            assert entry.filename.split('/')[0] in {f.name for f in folders}
            assert z.read(entry.filename) == (MODS/entry.filename).read_bytes()

def main():
    OUT.mkdir(exist_ok=True)
    folders = sorted(p for p in MODS.iterdir() if (p/'modinfo.json').is_file())
    for folder in folders:
        archive(OUT/(folder.name+'.zip'), [folder])
    archive(OUT/'all-examples.zip', folders)
    lines = [hashlib.sha256(p.read_bytes()).hexdigest()+'  '+p.name for p in sorted(OUT.glob('*.zip'))]
    (OUT/'SHA256SUMS.txt').write_text('\n'.join(lines)+'\n', encoding='utf-8', newline='\n')
    print(f'PASS {len(folders)+1} deterministic ZIPs: integrity, install paths, and every file verified.')

if __name__ == '__main__':
    main()
