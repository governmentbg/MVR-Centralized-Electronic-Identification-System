package bg.bulsi.mvr.raeicei.model;


import jakarta.persistence.*;
import lombok.Getter;
import lombok.Setter;
import org.springframework.data.annotation.CreatedBy;
import org.springframework.data.annotation.CreatedDate;
import org.springframework.data.annotation.LastModifiedBy;
import org.springframework.data.annotation.LastModifiedDate;
import org.springframework.data.jpa.domain.support.AuditingEntityListener;
import org.springframework.format.annotation.DateTimeFormat;

import java.io.Serial;
import java.io.Serializable;
import java.time.LocalDateTime;
import java.util.UUID;

@Getter
@Setter
@MappedSuperclass
@EntityListeners(AuditingEntityListener.class)
public abstract class AbstractAudit<U> implements Serializable {

    @Serial
    private static final long serialVersionUID = -3844176509415785042L;

    @CreatedBy
    @Column(name = "created_by", updatable = false)
    private U createdBy;

    @CreatedDate
    @Column(name = "create_date", updatable = false)
    @DateTimeFormat(pattern="dd-MM-yyyy HH:mm:ss")
    private LocalDateTime createDate = LocalDateTime.now();

    @LastModifiedBy
    @Column(name = "updated_by")
    private U updatedBy;

    @Version
    @Column(name = "version")
    private Long version;

    @LastModifiedDate
    @DateTimeFormat(pattern="dd-MM-yyyy HH:mm:ss")
    @Column(name = "last_update")
    private LocalDateTime lastUpdate;
}