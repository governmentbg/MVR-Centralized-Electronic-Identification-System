package bg.bulsi.mvr.mpozei.model.id_generator;

import java.io.Serializable;

import lombok.Data;
import lombok.NonNull;

@Data
public class NumberCounterId implements Serializable{

	private static final long serialVersionUID = -3002924979729245228L;

    @NonNull
	private String administratorCode;
    @NonNull
    private String officeCode;
    
	public NumberCounterId(@NonNull String administratorCode, @NonNull String officeCode) {
		super();
		this.administratorCode = administratorCode;
		this.officeCode = officeCode;
	}

	public NumberCounterId() {
		super();
	}
	
    
}
