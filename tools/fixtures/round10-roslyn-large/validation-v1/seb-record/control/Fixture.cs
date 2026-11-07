using System.Collections.Generic;
public class Fixture {
    public void Run(List<short[]> vector, int[] values) {
        short[] record = new short[7];
        record[0] = (short)values[0];
        record[1] = (short)values[1];
        record[2] = (short)values[2];
        record[3] = (short)values[3];
        record[4] = (short)values[4];
        record[5] = (short)values[5];
        record[6] = (short)values[6];
        vector.Add(record);
    }
}
