using UnityEngine;
using System.Collections;

public static class LayerMasks {
	private const string LAYER_NAME_OBSTACLES = "obstacles";

	public static LayerMask obstacles
	{
		get { return LayerMask.NameToLayer(LAYER_NAME_OBSTACLES); }
	}
}
