using System.Collections.Generic;
public class Fixture {
    public void Run(List<short[]> vector, int[] values) {
        vector.Add(new short[7] { (short)values[0], (short)values[1], (short)values[2], (short)values[3], (short)values[4], (short)values[5], (short)values[6] });
    }
}
