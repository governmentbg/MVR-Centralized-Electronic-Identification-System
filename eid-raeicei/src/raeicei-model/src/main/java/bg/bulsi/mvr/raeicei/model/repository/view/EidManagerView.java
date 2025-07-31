package bg.bulsi.mvr.raeicei.model.repository.view;

import java.io.Serializable;
import java.util.List;

import java.util.UUID;

import bg.bulsi.mvr.raeicei.contract.dto.ServiceType;
import bg.bulsi.mvr.raeicei.model.enums.ManagerType;

import jakarta.persistence.SqlResultSetMapping;
import jakarta.persistence.ConstructorResult;
import jakarta.persistence.ColumnResult;
import jakarta.persistence.EntityResult;
import jakarta.persistence.FieldResult;
import jakarta.persistence.MappedSuperclass;

@SqlResultSetMapping(name = "EidManagerViewMapping", entities = {
		@EntityResult(entityClass = EidManagerView.class, fields = { 
				@FieldResult(name = "id", column = "id"),
				@FieldResult(name = "name", column = "name"),
				@FieldResult(name = "nameLatin", column = "nameLatin"),
				@FieldResult(name = "eikNumber", column = "eikNumber"),
				@FieldResult(name = "address", column = "address"),
				@FieldResult(name = "homePage", column = "homePage"),
				@FieldResult(name = "managerStatus", column = "managerStatus"),
				@FieldResult(name = "logoUrl", column = "logoUrl"),
				@FieldResult(name = "downloadUrl", column = "downloadUrl"),
				@FieldResult(name = "code", column = "code"),
				@FieldResult(name = "eidManagerFrontOfficeIds", column = "eidManagerFrontOfficeIds"),
				@FieldResult(name = "eidFrontOffices", column = "eidFrontOffices"),
				@FieldResult(name = "deviceIds", column = "deviceIds")}) ,
		@EntityResult(entityClass = EIdFronOfficeView.class, fields = { 
				@FieldResult(name = "manager", column = "ffmid"),
				@FieldResult(name = "id", column = "ffid"),
				@FieldResult(name = "name", column = "ffname"),
				@FieldResult(name = "region", column = "ffregion") }) }
		)
@MappedSuperclass
public interface EidManagerView extends Serializable{
	abstract public UUID getId();

	abstract public String getName();

	abstract public String getNameLatin();

	abstract public String getEikNumber();

	abstract public String getAddress();

	abstract public String getEmail();

	abstract public String getHomePage();

	abstract public String getManagerStatus();

	abstract public String getLogoUrl();

	abstract public String getCode();

	abstract public List<UUID> getEidManagerFrontOfficeIds();

	abstract public String getEidFrontOffices();

	abstract public ServiceType getServiceType();
}
