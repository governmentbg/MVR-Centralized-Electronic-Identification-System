package bg.bulsi.mvr.audit_logger;

import java.io.IOException;
import java.nio.file.Files;
import java.nio.file.Paths;

import ch.qos.logback.core.rolling.RolloverFailure;
import ch.qos.logback.core.rolling.SizeAndTimeBasedRollingPolicy;
import ch.qos.logback.core.rolling.helper.CompressionMode;
import ch.qos.logback.core.rolling.helper.FileFilterUtil;
import ch.qos.logback.core.rolling.helper.RenameUtil;


/**
 * This class avoids renaming the live log file during rollover by copying its contents into a new archive file.
 * Compression is not supported.
 */
public class MoveWithoutRenameRollingPolicy<E> extends SizeAndTimeBasedRollingPolicy<E> {

	private RenameUtil renameUtil;
	
	private String archiveDirectory;
	
	
	public void setArchiveDirectory(String archiveDirectory) {
		this.archiveDirectory = archiveDirectory;
	}

	@Override
	public void start() {
	    super.start();
	    
	    this.renameUtil = new RenameUtil();
	    this.renameUtil.setContext(context);
	}

	@Override
	public void rollover() throws RolloverFailure {
        String elapsedPeriodsFileName = this.getTimeBasedFileNamingAndTriggeringPolicy().getElapsedPeriodsFileName();
        String elapsedPeriodStem = FileFilterUtil.afterLastSlash(elapsedPeriodsFileName);
        if (compressionMode == CompressionMode.NONE) {
        	 try {
				if (getParentsRawFileProperty() == null && Files.size(Paths.get(elapsedPeriodsFileName)) != 0) {
					 renameUtil.rename(elapsedPeriodsFileName, this.archiveDirectory + elapsedPeriodStem);
				 }
			} catch (RolloverFailure | IOException e) {
	            addWarn("Unable to open target file [" + elapsedPeriodsFileName + "]");
			}
        }
	}
}
