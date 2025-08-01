//
//  DebugLogger.swift
//  eID-Mobile-iOS
//
//  Created by Teodora Georgieva on 24.04.25.
//

import Foundation


class DebugLogger {
    static let shared: DebugLogger = {
        let logger = DebugLogger()
        logger.openConsolePipe()
        return logger
    }()
    
    static private var inputPipe = Pipe()
    static private var outputPipe = Pipe()
    static private var fileRedirectURL: URL?
    
    private init() { }
    
    func redirectOutputToFile(_ fileURL: URL) {
        DebugLogger.fileRedirectURL = fileURL
    }
    
    private func openConsolePipe() {
        // Route everything that comes in throught the outputPipe back to xcode console
        dup2(fileno(stdout), DebugLogger.outputPipe.fileHandleForWriting.fileDescriptor)
        
        // Route printing functions that go to sys pipes (stdout & stderr) into local inputPipe
        dup2(DebugLogger.inputPipe.fileHandleForWriting.fileDescriptor, fileno(stdout))
        dup2(DebugLogger.inputPipe.fileHandleForWriting.fileDescriptor, fileno(stderr))
        
        // Begin notification listener
        let inputReadHandler = DebugLogger.inputPipe.fileHandleForReading
        
        NotificationCenter.default.addObserver(self, selector: #selector(handlePipeNotification(notification:)), name: FileHandle.readCompletionNotification, object: inputReadHandler)
        
        inputReadHandler.readInBackgroundAndNotify()
    }
    
    @objc private func handlePipeNotification(notification: Notification) {
        DebugLogger.inputPipe.fileHandleForReading.readInBackgroundAndNotify()
        
        if let data = notification.userInfo?[NSFileHandleNotificationDataItem] as? Data {
            DebugLogger.outputPipe.fileHandleForWriting.write(data)
            
            if let fileURL = DebugLogger.fileRedirectURL {
                if FileManager.default.fileExists(atPath: fileURL.path) == false {
                    FileManager.default.createFile(atPath: fileURL.path, contents: nil, attributes: nil)
                }
                
                do {
                    try data.append(fileURL: fileURL)
                }  catch let error {
                    print(error)
                }
            }
        }
    }
}
