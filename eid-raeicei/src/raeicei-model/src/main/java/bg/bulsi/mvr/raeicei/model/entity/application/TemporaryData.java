package bg.bulsi.mvr.raeicei.model.entity.application;


import lombok.Data;

import java.util.List;
import bg.bulsi.mvr.raeicei.contract.dto.AuthorizedPersonel;

@Data
public class TemporaryData {

	private List<AuthorizedPersonel> guardians;

}
