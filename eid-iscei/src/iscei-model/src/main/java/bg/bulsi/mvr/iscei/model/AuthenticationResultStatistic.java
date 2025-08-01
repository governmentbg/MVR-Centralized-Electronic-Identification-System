package bg.bulsi.mvr.iscei.model;

import java.util.UUID;

import jakarta.persistence.Column;
import jakarta.persistence.DiscriminatorValue;
import jakarta.persistence.Entity;
import lombok.Getter;
import lombok.Setter;

@Getter
@Setter
@Entity
@DiscriminatorValue(value = "RESULT")
public class AuthenticationResultStatistic extends AuthenticationStatistic {

	/**
	 * 
	 */
	private static final long serialVersionUID = 2304755361836749685L;

	@Column
	private Boolean success;
}
