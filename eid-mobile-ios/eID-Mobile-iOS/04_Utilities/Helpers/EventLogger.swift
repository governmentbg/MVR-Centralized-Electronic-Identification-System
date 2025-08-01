//
//  EventLogger.swift
//  eID-Mobile-iOS
//
//  Created by Sunay Gyultekin on 18.10.24.
//

import SwiftUI


final class EventLogger: ObservableObject {
    // MARK: - Properties
    static let shared = EventLogger()
    
    // MARK: - Init
    static func logEvent(eventType: LogEventType, payload: LogEventPayload) {
        let request = LogEventRequest(eventType: eventType,
                                      eventPayload: payload)
        shared.logEvent(request)
    }

    private func logEvent(_ event: LogEventRequest) {
        ElectronicIdentityRouter.logEvent(input: event).send(String.self) { response in
            switch  response {
            case .success:
                print("Event logged: \(event.eventType.rawValue) (\(event.eventPayload))")
            case .failure(let error):
                print("Error logging event: \(error.localizedDescription)")
            }
        }
    }
}
