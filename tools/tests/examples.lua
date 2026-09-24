-- Lua behavior tests with fake C# objects. NOT xLua bridge or game acceptance.
local count = 0
local function check(value, message)
    assert(value, message)
    count = count + 1
end
local function list(values)
    local result = {Count = 0}
    function result:Add(value)
        self[self.Count] = value
        self.Count = self.Count + 1
    end
    for _, v in ipairs(values or {}) do result:Add(v) end
    return result
end
local registered, materials, sets, yakus, recipes, logs, styles = {}, {}, {}, {}, {}, {}, {}
local A = {Rarity = {COMMON=0, RARE=1, EPIC=2, LEGENDARY=3, ANCIENT=4}, Artifact = {}}
CS = {Aotenjo = A, System = {String='string', Int32='int', Array={}, Collections={Generic={}}}}
typeof = function(t) return t end
CS.System.Array.CreateInstance = function(t, length) return {Length=length, elementType=t} end
CS.System.Collections.Generic.List = function(t) return function() return list() end end
A.Logger = {Log=function(s) logs[#logs+1] = s end}
local function effect(kind, value, source, consume)
    local e = {kind=kind, value=value, source=source}
    function e:OnTile(tile) return {effect=self, tile=tile, animation=true} end
    function e:Ingest(player)
        if kind == 'money' then player.money = player.money + value end
        if kind == 'fu' then player.fu = player.fu + value end
        if kind == 'fan' then player.fan = player.fan + value end
        if kind == 'mult' then player.fan = player.fan * value end
        if consume then consume(player) end
    end
    return e
end
A.ScoreEffect = {
    AddFu=function(n,a) return effect('fu',n,a) end,
    AddFan=function(n,a) return effect('fan',n,a) end,
    MulFan=function(n,a) return effect('mult',n,a) end}
A.EarnMoneyEffect = function(n,a) return effect('money',n,a) end
A.SimpleEffect = function(key,a,f) return effect('simple',key,a,f) end

local function builder(id, rarity, registry)
    local b = {id=id, rarity=rarity, callbacks={}, data={}}
    local function build(self, craftable)
        local obj = {id=self.id, rarity=self.rarity, callbacks=self.callbacks, data={}, craftable=craftable}
        for k,v in pairs(self.data) do obj.data[k]=v end
        function obj:GetRegName() return self.id end
        function obj:GetDataOrDefault(k,d)
            check(type(d)=='string', 'state default must be a string')
            return self.data[k] or d
        end
        function obj:SetData(k,v)
            check(type(v)=='string', 'state value must be a string')
            self.data[k]=v
        end
        return obj
    end
    function b:WithRarity(r) self.rarity=r; return self end
    function b:WithData(k,v) self.data[k]=v; return self end
    function b:Build() return build(self,false) end
    function b:BuildAndRegister()
        check(registry[self.id]==nil, 'duplicate registration: '..self.id)
        local obj=build(self,false); registry[self.id]=obj; return obj
    end
    function b:BuildAndRegisterCraftable()
        local obj=self:BuildAndRegister(); obj.craftable=true; return obj
    end
    return setmetatable(b,{__index=function(self,key)
        return function(self,callback) self.callbacks[key]=callback; return self end
    end})
end
A.LuaArtifactBuilder={Create=function(id,r) return builder(id,r,registered) end}
A.LuaTileMaterialBuilder={Create=function(id) return builder(id,0,materials) end}
A.LuaMaterialSetBuilder={Create=function(id)
    local s={id=id, items={}}
    function s:AddMaterial(m) self.items[#self.items+1]=m; return self end
    function s:BuildAndRegister() sets[self.id]=self; return self end
    return s
end}
A.CustomYakuBuilder={RegisterCustomYaku=function(id,base,growth,level,predicate,included,groups,categories,rarity,example)
    check(yakus[id]==nil, 'duplicate yaku')
    check(included.Length==0 and included.elementType=='string', 'typed empty inheritance array')
    check(groups.Length==2 and groups[0]=='standard' and groups[1]=='galaxy', 'zero based groups')
    check(categories.Length==1 and categories[0]==2 and categories.elementType=='int', 'Fire pack index')
    check(base==8 and growth==1.5 and level==3 and rarity==0, 'yaku balance parameters')
    yakus[id]={predicate=predicate,example=example}
end}
A.LuaArtifactRecipeBuilder={BuildAndRegister=function(id,inputs,result)
    check(inputs.Count==2, 'recipe uses C# style list with two inputs')
    check(result.craftable, 'recipe output must be craftable')
    local r={id=id, inputs=inputs, result=result}; recipes[id]=r; return r
end}
A.TileFaceMaterialRegistry={
    RegisterHex=function(id,color) styles[id]=color; return true end,
    RegisterRainbow=function(id) styles[id]='rainbow'; return true end}

-- Simulate the loader's shared package.path and global init semantics.
local folders={'00_hello','ex1_artifact','ex2_pattern','ex3_tile_material','ex4_recipe','ex6_face_colors'}
for _,folder in ipairs(folders) do
    local path='example/mods/'..folder..'/script/'
    package.path=package.path..';'..path..'?.lua'
    assert(loadfile(path..'init.lua'))()
    check(type(init)=='function', folder..' defines global init')
    init()
end
check(#logs==6, 'one success marker per script mod')
local function tile(n,selected)
    return {number=n, selected=selected, IsNumbered=function(self,want)
        return self.number>=1 and self.number<=9 and (want==nil or self.number==want)
    end}
end
local player={money=0,fu=0,fan=10,Selecting=function(self,t) return t.selected end}
local coin=registered['tutorial_artifact:coin_twos']
for _,case in ipairs({{2,true,1},{2,false,0},{3,true,0},{0,true,0},{9,false,0}}) do
    local effects=list()
    coin.callbacks.OnTileEffect(player,nil,tile(case[1],case[2]),effects,coin)
    check(effects.Count==case[3], 'coin condition '..tostring(case[1])..' '..tostring(case[2]))
    check(player.money==0, 'queue construction must not award money')
    if effects.Count==1 then
        check(effects[0].source==coin and effects[0].value==2, 'coin amount and source')
        effects[0]:Ingest(player)
        check(player.money==2, 'money awarded on execution')
        player.money=0
    end
end
local jade=registered['tutorial_artifact:practice_jade']
jade.callbacks.ResetArtifactState(player,jade)
local loc=function(key) return 'Fan x%.1f' end
check(jade.callbacks.WithDescription(nil,loc,jade)=='Fan x1.0','nil-player collection description')
for turn=0,2 do
    local effects=list()
    jade.callbacks.OnSelfEffect(player,nil,effects,jade)
    check(effects.Count==2, 'multiplier plus deferred growth')
    check(effects[0].value==1+turn*0.1, 'multiplier reflects previous executions')
    check(jade.data.plays==tostring(turn), 'no growth during queue construction')
    effects[1]:Ingest(player)
    check(jade.data.plays==tostring(turn+1), 'growth on effect execution')
end
local before=jade.data.plays
jade.callbacks.WithDescription(nil,loc,jade)
jade.callbacks.WithDescription(player,loc,jade)
check(jade.data.plays==before,'description queries have no side effects')
jade.callbacks.ResetArtifactState(player,jade)
check(jade.data.plays=='0','new run reset')

local yaku=yakus['tutorial_yaku:all_twos']
for _,case in ipairs({{{},false},{{2},true},{{2,2,2},true},{{2,3},false},{{2,0},false}}) do
    local tiles=list()
    for _,n in ipairs(case[1]) do tiles:Add(tile(n,true)) end
    check(yaku.predicate({ToTiles=function() return tiles end},player)==case[2], 'yaku positive/negative/empty case')
end
local ruby=materials['tutorial_gems:ruby']
local sapphire=materials['tutorial_gems:sapphire']
check(ruby and sapphire,'registered material IDs')
local t=tile(2,true)
local e=list(); ruby.callbacks.OnScoringEffect(player,nil,t,e,ruby)
check(e.Count==1 and e[0].kind=='fu' and e[0].value==20, 'Ruby adds 20 Fu')
e=list(); sapphire.callbacks.OnScoringEffect(player,nil,t,e,sapphire)
check(e.Count==1 and e[0].kind=='mult' and e[0].value==2, 'Sapphire doubles Fan')
e=list(); ruby.callbacks.OnRoundEndEffect(player,nil,e,t,ruby)
check(e.Count==1 and e[0].animation and e[0].tile==t, 'round-end uses target animation wrapper')
check(e[0].effect.kind=='money' and e[0].effect.value==2,'round-end reward matches translations')
check(#sets.tutorial_gemstones.items==4,'set has all four materials')
local r=recipes['tutorial_recipe:tokens']
check(r.inputs[0].id=='tutorial_recipe:copper_token' and r.inputs[1].id=='tutorial_recipe:silver_token', 'recipe exact inputs')
check(r.result.id=='tutorial_recipe:double_token','recipe exact output')
e=list(); r.result.callbacks.OnSelfEffect(player,nil,e,r.result)
check(e.Count==1 and e[0].kind=='fan' and e[0].value==5,'crafted output adds 5 Fan')
check(styles.plain=='#24CFA6' and styles.blue=='rainbow','face style targets and values')
print('PASS '..count..' Lua scenario assertions (fake CS objects; not in-game or C# binding validation)')
