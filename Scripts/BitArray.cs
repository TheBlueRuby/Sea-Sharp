using System.Diagnostics;

[DebuggerDisplay("{PrintArray(), nq}")]
public class BitArray {
	private int array;

	public BitArray() {
		array = 0;
	}

	/// <summary>
	/// Checks if a specific bit is set in the array.
	/// </summary>
	/// <param name="bit">The bit to check.</param>
	/// <returns>True if the bit is set, otherwise false.</returns>
	public bool CheckBit(int pos) {
		return (array & (1 << pos)) != 0;
	}

	/// <summary>
	/// Checks if a specific bit is set in the array.
	/// Assumes pos is a power of 2
	/// </summary>
	/// <param name="bit">The bit to check as a power of 2.</param>
	/// <returns>True if the bit is set, otherwise false.</returns>
	public bool CheckBitPow(int pos) {
		return (array & pos) != 0;
	}

	/// <summary>
	/// Sets the value of a specific bit in the BitArray.
	/// </summary>
	/// <param name="bit">The index of the bit to set.</param>
	/// <param name="on">The value to set the bit to. True for 1, false for 0.</param>
	public void SetBitVal(int bit, bool on) {
		if (on) {
			SetBit(bit);
		} else {
			ClearBit(bit);
		}
	}

	/// <summary>
	/// Sets the specified bit in the BitArray.
	/// </summary>
	/// <param name="bit">The bit to set.</param>
	public void SetBit(int bit) {
		array |= bit;
	}

	/// <summary>
	/// Clears the specified bit in the BitArray.
	/// </summary>
	/// <param name="bit">The index of the bit to clear.</param>
	public void ClearBit(int bit) {
		array &= ~array | bit;
	}

	public string PrintArray() {
		string output = "";
		for (int i = 0; i < 32; i++) {
			output += CheckBit(i) ? "1" : "0";
		}
		return output;
	}

	public static BitArray FromString(string stringRep) {
		BitArray output = new BitArray();
		for (int i = 0; i < 32; i++) {
			if (stringRep[i] == '1') {
				output.SetBit(2^i);
			}
		}
		return output;
	}

}
