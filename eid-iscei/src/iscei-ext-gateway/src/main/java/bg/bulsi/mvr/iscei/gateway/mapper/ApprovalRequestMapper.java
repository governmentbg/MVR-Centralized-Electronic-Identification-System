package bg.bulsi.mvr.iscei.gateway.mapper;

import java.time.Instant;
import java.util.List;

import org.mapstruct.AfterMapping;
import org.mapstruct.Mapper;
import org.mapstruct.MappingTarget;
import org.mapstruct.NullValueCheckStrategy;
import org.springframework.stereotype.Component;

import bg.bulsi.mvr.iscei.contract.dto.approvalrequest.ApprovalRequestResponse;
import bg.bulsi.mvr.iscei.model.AuthApprovalRequest;


@Component
@Mapper(componentModel = "spring",
        nullValueCheckStrategy = NullValueCheckStrategy.ALWAYS)
public abstract class ApprovalRequestMapper {

    //@Mapping(source = "id", target = "eidentityId")
    public abstract List<ApprovalRequestResponse> map(List<AuthApprovalRequest> authApprovalRequests);
    
    public abstract ApprovalRequestResponse map(AuthApprovalRequest authApprovalRequest);
    
    @AfterMapping
    protected void setExpiresIn(AuthApprovalRequest authApprovalRequest, @MappingTarget ApprovalRequestResponse approvalRequestResponse) {
    	long expiresIn = Math.abs(Instant.now().getEpochSecond() - (authApprovalRequest.getCreateDate().toEpochSecond() + authApprovalRequest.getMaxTtl()));
    	approvalRequestResponse.setExpiresIn(expiresIn);
    }
}
