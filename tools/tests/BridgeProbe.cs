// Real xLua bridge + real exported Lua builders/instances.
// The surrounding game/Unity types below are deliberately minimal test doubles.
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using Aotenjo;
using XLua;

namespace UnityEngine {
    public class SerializeField : Attribute { }
    public static class Debug { public static void LogWarning(object o) => Console.WriteLine(o); }
}
namespace Unity.VisualScripting { public class NamespaceMarker { } }
namespace Sirenix.OdinInspector { public class LabelTextAttribute : Attribute { public LabelTextAttribute(string label) { } } }

public class LotteryPool<T> { }
public class Yaku {
    public YakuType[] includedYakus;
    public Yaku(YakuType type, int fan, double growth, double leveling, YakuType[] included, string[] groups, Rarity rarity, string example, int[] categories) { includedYakus=included; }
}
namespace Aotenjo {
    public static class Logger { public static void Log(string s) => Console.WriteLine(s); public static void LogError(string s) => throw new Exception(s); }
    public class SerializableMap<K,V> : Dictionary<K,V> {
        public V Get(K k) => TryGetValue(k,out var v)?v:default;
        public SerializableMap<K,V> Clone() { var copy=new SerializableMap<K,V>(); foreach(var kv in this) copy[kv.Key]=kv.Value; return copy; }
    }
    public class Player {
        public int Money;
        public bool Selecting(Tile tile) => tile.Selected;
        public List<Artifact> GetArtifacts() => new();
        public bool RemoveArtifact(Artifact a, bool reset, bool shuffle) => true;
        public bool ObtainArtifact(Artifact a) => true;
    }
    public class Tile { public int Number; public bool Selected; public bool IsNumbered(int n) => Number==n && n>=1 && n<=9; }
    public class Block { }
    public class Permutation { public List<Tile> Tiles=new(); public List<Tile> ToTiles()=>Tiles; }
    public interface IAnimationEffect { }
    public class Effect : IAnimationEffect {
        public string Kind; public double Value; public Artifact Source;
        public virtual void Ingest(Player p) { if(Kind=="money") p.Money+=(int)Value; }
        public AnimationEffect OnTile(Tile tile) => new AnimationEffect { Effect=this, Tile=tile };
    }
    public class AnimationEffect : IAnimationEffect { public Effect Effect; public Tile Tile; }
    public class ScoreEffect : Effect {
        public static ScoreEffect AddFu(double v, Artifact a) => new() { Kind="fu",Value=v,Source=a };
        public static ScoreEffect AddFan(double v, Artifact a) => new() { Kind="fan",Value=v,Source=a };
        public static ScoreEffect MulFan(double v, Artifact a) => new() { Kind="mult",Value=v,Source=a };
    }
    public class EarnMoneyEffect : Effect { public EarnMoneyEffect(int n,Artifact a) { Kind="money";Value=n;Source=a; } }
    public class SimpleEffect : Effect { private Action<Player> consume; public SimpleEffect(string key,Artifact a,Action<Player> f,string sound="AddFu") { consume=f;Source=a;Kind=key; } public override void Ingest(Player p)=>consume(p); }
    public partial class Artifact {
        private string name;
        public bool IsBroken, IsTemporary;
        public List<string> deckIn,deckBlocked,setIn,setBlocked,materialRequired;
        public Artifact(string name,Rarity rarity) { this.name=name; }
        public string GetRegName()=>name;
        public string GetNameID()=>name;
    }
    public class CraftableArtifact : Artifact { public CraftableArtifact(string name,Rarity rarity):base(name,rarity) { } }
    public static class Artifacts { public static List<Artifact> ArtifactList=new(); public static Artifact GetArtifact(string id)=>ArtifactList.First(a=>a.GetRegName()==id); }
    public static class ArtifactRecipes { public static List<ArtifactRecipe> recipes=new(); }
    public partial class TileMaterial {
        private string name;
        public TileMaterial(int id,string name,Effect effect) { this.name=name+"_material"; }
        public string GetRegName()=>name;
        protected virtual string GetSpriteNamespaceID(Player p,string nmSpace="aotenjo")=>name;
        public static Func<TileMaterial>[] MaterialProviders=Array.Empty<Func<TileMaterial>>();
        public static TileMaterial GetMaterial(string id) => MaterialProviders.Select(f=>f()).First(m=>m.GetRegName()==(id.EndsWith("_material")?id:id+"_material"));
    }
    public partial class MaterialSet {
        private string name;
        public static MaterialSet[] MaterialSets=Array.Empty<MaterialSet>();
        public MaterialSet(int id,string name,List<string> materials) { this.name=name; }
        public string GetRegName()=>name;
    }
    public class YakuPack { public List<YakuType> common=new(), rare=new(), epic=new(), legendary=new(), ancient=new(); }
    public class RegisterManager { public static RegisterManager Instance=new(); public List<YakuPack> YakuPacks=Enumerable.Range(0,16).Select(_=>new YakuPack()).ToList(); }
    public static class YakuTester { public static Dictionary<YakuType,Yaku> InfoMap=new(); public static Dictionary<YakuType,Func<Permutation,Player,bool>> YAKUS_PREDICATE_MAP=new(); }
    public static class TileFaceMaterialRegistry { public static bool RegisterHex(string id,string color)=>true; public static bool RegisterRainbow(string id)=>true; }
}

// Explicit bridge implementations for signatures over four arguments, matching
// signatures already present in the game's generated bridge. Short reference-only
// callbacks exercise xLua's real generic delegate fallback.
namespace XLua {
    public partial class DelegateBridge {
        private bool InvokeTyped(bool returnsBool, params object[] args) {
            var L=luaEnv.L;
            int oldTop=LuaDLL.Lua.lua_gettop(L);
            try {
                int err=LuaDLL.Lua.load_error_func(L,errorFuncRef);
                LuaDLL.Lua.lua_getref(L,luaReference);
                foreach(var arg in args) luaEnv.translator.PushAny(L,arg);
                PCall(L,args.Length,returnsBool?1:0,err);
                return returnsBool && LuaDLL.Lua.lua_toboolean(L,-1);
            } finally { LuaDLL.Lua.lua_settop(L,oldTop); }
        }
        private void TileCallback(Player p,Permutation perm,Tile t,List<Effect> e,Artifact a) {
            InvokeTyped(false,p,perm,t,e,a);
        }
        private void MaterialCallback(Player p,Permutation perm,Tile t,List<Effect> e,TileMaterial m) {
            InvokeTyped(false,p,perm,t,e,m);
        }
        private void RoundCallback(Player p,Permutation perm,List<IAnimationEffect> e,Tile t,TileMaterial m) {
            InvokeTyped(false,p,perm,e,t,m);
        }
        private bool Highlight(Tile t,Player p,Artifact a) {
            return InvokeTyped(true,t,p,a);
        }
        private bool Predicate(Permutation perm,Player p) {
            return InvokeTyped(true,perm,p);
        }
        public override Delegate GetDelegateByType(Type t) {
            if(t==typeof(Action<Player,Permutation,Tile,List<Effect>,Artifact>)) return new Action<Player,Permutation,Tile,List<Effect>,Artifact>(TileCallback);
            if(t==typeof(Action<Player,Permutation,Tile,List<Effect>,TileMaterial>)) return new Action<Player,Permutation,Tile,List<Effect>,TileMaterial>(MaterialCallback);
            if(t==typeof(Action<Player,Permutation,List<IAnimationEffect>,Tile,TileMaterial>)) return new Action<Player,Permutation,List<IAnimationEffect>,Tile,TileMaterial>(RoundCallback);
            if(t==typeof(Func<Tile,Player,Artifact,bool>)) return new Func<Tile,Player,Artifact,bool>(Highlight);
            if(t==typeof(Func<Permutation,Player,bool>)) return new Func<Permutation,Player,bool>(Predicate);
            return null;
        }
    }
}

public static class BridgeProbe {
    private static int checks;
    private static void Check(bool value,string why) { if(!value) throw new Exception(why); checks++; }
    public static int Main(string[] args) {
        try {
            NativeLibrary.SetDllImportResolver(typeof(LuaEnv).Assembly,(name,assembly,path)=>name=="xlua"?NativeLibrary.Load(args[1]):IntPtr.Zero);
            var lua=new LuaEnv();
            foreach(var folder in new[]{"00_hello","ex1_artifact","ex2_pattern","ex3_tile_material","ex4_recipe","ex6_face_colors"}) {
                var root=Path.Combine(args[0],"example","mods",folder,"script").Replace('\\','/');
                lua.DoString("package.path = package.path .. ';"+root+"/?.lua'");
                lua.DoString(File.ReadAllText(root+"/init.lua"),folder);
                lua.DoString("init()");
                checks++;
            }
            var p=new Player(); var tile=new Tile{Number=2,Selected=true};
            var coin=Artifacts.GetArtifact("tutorial_artifact:coin_twos");
            var e=new List<Effect>(); coin.AppendOnTileEffects(p,null,tile,e);
            Check(e.Count==1 && e[0].Value==2,"typed tile callback");
            e[0].Ingest(p); Check(p.Money==2,"queued money");
            e.Clear();tile.Selected=false;coin.AppendOnTileEffects(p,null,tile,e);Check(e.Count==0,"selection negative");
            var jade=(LuaArtifact)Artifacts.GetArtifact("tutorial_artifact:practice_jade");
            jade.ResetArtifactState(p);e.Clear();jade.AppendOnSelfEffects(p,null,e);
            Check(e.Count==2 && e[0].Value==1,"typed self callback");e[1].Ingest(p);
            Check(jade.GetDataOrDefault("plays","0")=="1","typed nested Action<Player>");
            var saved=jade.Serialize();jade.SetData("plays","99");jade.Deserialize(saved);
            Check(jade.GetDataOrDefault("plays","0")=="1","real LuaArtifact JSON roundtrip");
            Check(jade.GetDescription((Player)null,key=>"Fan x%.1f")=="Fan x1.1","nil-player localizer delegate");
            var type=YakuType.FromString("custom_yaku:tutorial_yaku:all_twos");
            Check(RegisterManager.Instance.YakuPacks[2].common.Contains(type),"real yaku pack registration");
            var perm=new Permutation();Check(!YakuTester.YAKUS_PREDICATE_MAP[type](perm,p),"typed predicate empty");
            perm.Tiles.Add(new Tile{Number=2});Check(YakuTester.YAKUS_PREDICATE_MAP[type](perm,p),"typed predicate positive");
            var ruby=(LuaTileMaterial)TileMaterial.GetMaterial("tutorial_gems:ruby");
            ruby.SetData("test","first");var copy=(LuaTileMaterial)ruby.Copy();copy.SetData("test","second");
            Check(ruby.GetDataOrDefault("test","")=="first","real material Copy isolates data");
            var ae=new List<IAnimationEffect>();ruby.AppendToListRoundEndEffect(p,null,ae,tile);
            Check(ae.Count==1 && ((AnimationEffect)ae[0]).Effect.Value==2,"typed round-end material callback");
            Check(ArtifactRecipes.recipes.Count==1 && ArtifactRecipes.recipes[0].inputID.Count==2,"real recipe builder and generic List<Artifact>");
            Console.WriteLine($"PASS {checks} real xLua/C# bridge checks with exported API implementations and stub game base types.");
            // Registry delegates intentionally live until this short-lived process exits.
            GC.KeepAlive(lua);
            return 0;
        } catch(Exception e) { Console.Error.WriteLine(e); return 1; }
    }
}
