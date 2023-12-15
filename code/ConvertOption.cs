using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public enum ReduceOption {
	None,
	DummySpinner,
};

public class ConvertOption {
	public int pixelError = 5;

	public bool findMoreRoutes = false;

	public ReduceOption reduceOption = ReduceOption.None;

	public int dummySpinnerStartTime = 0;

	public ConvertOption(int pixelError, bool findMoreRoutes, ReduceOption reduceOption, int dummySpinnerStartTime=0) {
		this.pixelError = pixelError;
		this.findMoreRoutes = findMoreRoutes;
		this.reduceOption = reduceOption;
		this.dummySpinnerStartTime = dummySpinnerStartTime;
	}
}
