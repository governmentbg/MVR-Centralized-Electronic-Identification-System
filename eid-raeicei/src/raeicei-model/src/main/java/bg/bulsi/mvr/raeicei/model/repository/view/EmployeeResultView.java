package bg.bulsi.mvr.raeicei.model.repository.view;

import java.io.Serializable;
import java.util.UUID;

import bg.bulsi.mvr.raeicei.contract.dto.IdentifierTypeDTO;
import jakarta.persistence.EntityResult;
import jakarta.persistence.FieldResult;
import jakarta.persistence.MappedSuperclass;
import jakarta.persistence.SqlResultSetMapping;
import lombok.AllArgsConstructor;
import lombok.Data;
import lombok.NoArgsConstructor;
import lombok.ToString;

public interface EmployeeResultView {

	abstract public String getUid();
	abstract public void setUid(String uid);

	abstract public IdentifierTypeDTO getUidType();
	abstract public void setUidType(IdentifierTypeDTO uidType);

	abstract public UUID getProviderId();
	abstract public void setProviderId(UUID providerId);

	abstract public String getProviderName();
	abstract public void setProviderName(String providerName);

	abstract public Boolean getIsAdministrator();
	abstract public void setIsAdministrator(Boolean isAdministrator);
}
