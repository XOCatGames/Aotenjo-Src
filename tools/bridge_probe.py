"""Developer-only typed xLua smoke test using real exported API implementations.

Requires .NET 10 and the matching developer Unity checkout for xLua and Newtonsoft.
Game base types/rendering are explicit test doubles; this is not game acceptance.
"""
import argparse
from pathlib import Path
import re
import subprocess
import tempfile
from xml.sax.saxutils import escape
from build_reference import declarations

ROOT = Path(__file__).resolve().parents[1]

def base_methods(path):
    methods = []
    for _, signature in declarations(path.read_text(encoding='utf-8-sig')):
        if ' override ' not in signature:
            continue
        signature = signature.replace(' override ', ' virtual ')
        body = '{}' if signature.startswith('public virtual void ') else '{ return default; }'
        methods.append(signature + ' ' + body)
    return '\n'.join(methods)

def main():
    parser = argparse.ArgumentParser()
    parser.add_argument('--unity-root', type=Path, required=True)
    args = parser.parse_args()
    unity = args.unity_root.resolve()
    stage = Path(tempfile.mkdtemp(prefix='Aotenjo-ModdingBridge-'))
    base = ROOT/'src/API'
    artifact_methods = base_methods(base/'Artifact/LuaArtifact.cs')
    material_methods = base_methods(base/'TileProperties/LuaTileMaterial.cs')
    set_methods = base_methods(base/'TileProperties/LuaMaterialSet.cs')
    generated = '''using System; using System.Collections.Generic; using System.Linq;
namespace Aotenjo {
public partial class Artifact { ARTIFACT_METHODS }
public partial class TileMaterial { MATERIAL_METHODS }
public partial class MaterialSet { SET_METHODS }
}
'''.replace('ARTIFACT_METHODS', artifact_methods).replace('MATERIAL_METHODS', material_methods).replace('SET_METHODS', set_methods)
    (stage/'Bases.cs').write_text(generated, encoding='utf-8')
    sources = list((unity/'Assets/XLua/Src').glob('*.cs'))
    sources += list((unity/'Assets/XLua/Src/TemplateEngine').glob('*.cs'))
    sources += list((base/'Artifact').glob('*.cs')) + list((base/'TileProperties').glob('*.cs'))
    sources += [base/'Yaku/CustomYakuBuilder.cs', ROOT/'src/ArtifactRecipe/ArtifactRecipe.cs',
                ROOT/'src/HandAndTile/Yaku/YakuType.cs', ROOT/'src/HandAndTile/Yaku/FixedYakuType.cs',
                ROOT/'src/Utils/Rarity.cs', ROOT/'tools/tests/BridgeProbe.cs', stage/'Bases.cs']
    json_dll = next((unity/'Library/PackageCache').glob('com.unity.nuget.newtonsoft-json*/Runtime/Newtonsoft.Json.dll'))
    items = ''.join(f'<Compile Include="{escape(str(p))}" />' for p in sources)
    items += f'<Reference Include="Newtonsoft.Json"><HintPath>{escape(str(json_dll))}</HintPath></Reference>'
    project = '<Project Sdk="Microsoft.NET.Sdk"><PropertyGroup><OutputType>Exe</OutputType><TargetFramework>net10.0</TargetFramework><EnableDefaultCompileItems>false</EnableDefaultCompileItems><AllowUnsafeBlocks>true</AllowUnsafeBlocks><DefineConstants>XLUA_GENERAL;NET_STANDARD_2_0</DefineConstants><NoWarn>CS8981;CS0108;CS0649;CS0169;CS0618;SYSLIB0050;SYSLIB0003;CS0168;CS0414;CA2200</NoWarn></PropertyGroup><ItemGroup>'+items+'</ItemGroup></Project>'
    (stage/'Probe.csproj').write_text(project, encoding='utf-8')
    # Native xLua and its generated delegate fallback, without Unity engine calls.
    command = ['dotnet','run','--project',str(stage/'Probe.csproj'),'--verbosity','quiet','--',str(ROOT),str(unity/'Assets/Plugins/x86_64/xlua.dll')]
    result = subprocess.run(command, cwd=ROOT)
    print('Probe workspace:', stage)
    raise SystemExit(result.returncode)

if __name__ == '__main__':
    main()
