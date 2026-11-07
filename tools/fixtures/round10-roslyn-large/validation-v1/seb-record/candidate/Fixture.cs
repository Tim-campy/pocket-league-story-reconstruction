using System.Collections.Generic;
public class Fixture {
    public void Run(List<short[]> vector, int[] values) {
        short[] recoveredRecord = new short[7];
recoveredRecord[0] = (short)values[0];
recoveredRecord[1] = (short)values[1];
recoveredRecord[2] = (short)values[2];
recoveredRecord[3] = (short)values[3];
recoveredRecord[4] = (short)values[4];
recoveredRecord[5] = (short)values[5];
recoveredRecord[6] = (short)values[6];
vector.Add(recoveredRecord);
    }
}
