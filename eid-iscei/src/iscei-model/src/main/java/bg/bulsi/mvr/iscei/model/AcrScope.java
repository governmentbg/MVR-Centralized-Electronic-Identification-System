package bg.bulsi.mvr.iscei.model;

public enum AcrScope {
	EID_LOA_HIGH("eid_loa_high"), 
	EID_LOA_SUBSTANTIAL("eid_loa_substantial"), 
	EID_LOA_LOW("eid_loa_low");

	private final String acrScopeName;

	private AcrScope(String acrScopeName) {
		this.acrScopeName = acrScopeName;
	}

	/**
	 * @return the acrScopeName
	 */
	public String getAcrScopeName() {
		return acrScopeName;
	}

}
