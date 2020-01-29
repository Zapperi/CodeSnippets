using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Author: Henri Leppänen
 * <summary>
 * A class for transfering and saving sticker information.
 * </summary>
 */

[CreateAssetMenu(fileName = "Sticker", order = 0)]
public class Sticker : ScriptableObject
{
    /// <summary>
    /// WorldID of the world associated to this sticker.
    /// </summary>
    public WorldManager.WorldID world;
    /// <summary>
    /// Sticker image sprite.
    /// </summary>
    public Sprite icon;
    /// <summary>
    /// Is the sticker repeatable?
    /// </summary>
    public bool repeatable = false;
}
