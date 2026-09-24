using System;
using System.Collections.Generic;
using Aotenjo;

/// <summary>
/// Outcome of a multi-tile use. Successful uses may have no animation tiles;
/// animation tiles need not be the selected tiles. This value never consumes uses.
/// </summary>
public readonly struct GadgetUseResult
{
    private readonly IReadOnlyList<Tile> animationTiles;

    public bool Success { get; }
    public IReadOnlyList<Tile> AnimationTiles => animationTiles ?? Array.Empty<Tile>();

    public static GadgetUseResult Failed => default;

    private GadgetUseResult(IReadOnlyList<Tile> tiles)
    {
        Success = true;
        // Preserve tile identity, but do not retain a mutable UI selection list.
        animationTiles = tiles == null ? Array.Empty<Tile>() : new List<Tile>(tiles).AsReadOnly();
    }

    public static GadgetUseResult Succeeded(IReadOnlyList<Tile> tiles = null) => new GadgetUseResult(tiles);

    public static GadgetUseResult FromSuccess(bool success, IReadOnlyList<Tile> tiles = null) =>
        success ? Succeeded(tiles) : Failed;
}
