using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public class BananaTable {
	public List<BananaInfo> infos;

	public BananaTable() {
		infos = new List<BananaInfo>();

		infos.Add(new BananaInfo(8, 1));

		int bananaCount = 3;
		int spinnerLength = 100;

		while (spinnerLength < 1000000) {
			infos.Add(new BananaInfo(bananaCount * 4, spinnerLength + 1));

			bananaCount = bananaCount * 2 - 1;
			spinnerLength *= 2;
		}
	}

	public List<BananaInfo> GetAvailableInfos(int rngCount) {
		List<BananaInfo> results = new List<BananaInfo>();

		for (int i = infos.Count - 1; i >= 0; i--) {
			BananaInfo info = infos[i];

			while (rngCount >= info.rngCount) {
				results.Add(info);
				rngCount -= info.rngCount;
			}
		}

		return results;
	}

	public class BananaInfo {
		public int rngCount;
		public int spinnerLength;

		public BananaInfo(int rngCount, int spinnerLength) {
			this.rngCount = rngCount;
			this.spinnerLength = spinnerLength;
		}
	}
}

