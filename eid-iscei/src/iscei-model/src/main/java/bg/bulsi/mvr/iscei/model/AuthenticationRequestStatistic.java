package bg.bulsi.mvr.iscei.model;

import jakarta.persistence.DiscriminatorValue;
import jakarta.persistence.Entity;
import lombok.AllArgsConstructor;
import lombok.Builder;
import lombok.Getter;
import lombok.NoArgsConstructor;
import lombok.Setter;

@Getter
@Setter
@Entity
@DiscriminatorValue(value = "REQUEST")
public class AuthenticationRequestStatistic extends AuthenticationStatistic {

	/**
	 * 
	 */
	private static final long serialVersionUID = 5125874599778194456L;

}
