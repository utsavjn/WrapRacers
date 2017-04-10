using UnityEngine;

public static class LayerMaskShifter
{
	public static int IntToLayerMask(int integer)
	{
		return 1 << integer;
	}
}
