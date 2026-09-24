"""Run Lua scenarios using an existing Lua/xLua native library (not a C# bridge).

python tools/run_lua_tests.py --library ../Aotenjo-Unity/Assets/Plugins/x86_64/xlua.dll
Alternatively, from repository root: lua tools/tests/examples.lua
The tests explicitly use fake CS game objects. They are not Play Mode tests.
"""
import argparse
import ctypes
import os
from pathlib import Path

ROOT = Path(__file__).resolve().parents[1]

def main():
    parser = argparse.ArgumentParser()
    parser.add_argument('--library', type=Path, required=True)
    args = parser.parse_args()
    lib = ctypes.CDLL(str(args.library.resolve()))
    lib.luaL_newstate.restype = ctypes.c_void_p
    lib.luaL_openlibs.argtypes = [ctypes.c_void_p]
    lib.luaL_loadstring.argtypes = [ctypes.c_void_p, ctypes.c_char_p]
    lib.luaL_loadstring.restype = ctypes.c_int
    lib.lua_pcall.argtypes = [ctypes.c_void_p, ctypes.c_int, ctypes.c_int, ctypes.c_int]
    lib.lua_pcall.restype = ctypes.c_int
    lib.lua_tolstring.argtypes = [ctypes.c_void_p, ctypes.c_int, ctypes.POINTER(ctypes.c_size_t)]
    lib.lua_tolstring.restype = ctypes.c_char_p
    lib.lua_close.argtypes = [ctypes.c_void_p]
    os.chdir(ROOT)
    state = lib.luaL_newstate()
    if not state:
        raise RuntimeError('Unable to allocate Lua state')
    try:
        lib.luaL_openlibs(state)
        code = (ROOT / 'tools/tests/examples.lua').read_bytes()
        status = lib.luaL_loadstring(state, code)
        if not status:
            status = lib.lua_pcall(state, 0, 0, 0)
        if status:
            raise RuntimeError(lib.lua_tolstring(state, -1, None).decode('utf-8', errors='replace'))
    finally:
        lib.lua_close(state)

if __name__ == '__main__':
    main()
