//
//  NotificationTypesViewModel.swift
//  eID-Mobile-iOS
//
//  Created by Sunay Gyultekin on 11.10.23.
//

import Foundation
import Alamofire


final class NotificationTypesViewModel: ObservableObject, APICallerViewModel {
    // MARK: - Properties
    private var notificationTypes: [NotificationTypeResponse] = []
    private var deactivatedEvents: [DeactivatedNotificationEvent] = []
    @Published var notificationTypesDisplayModels: [NotificationTypeDisplayModel] = []
    @Published var showLoading: Bool = false
    @Published var showError: Bool = false
    @Published var errorText: String = ""
    @Published private var pageIndex = 1
    @Published private var totalItemsCount = 0
    
    // MARK: - API calls
    func getNotificationTypes() {
        let request = GetNotificationTypesRequest(pageIndex: pageIndex)
        showLoading = true
        NotificationSettingsRouter.getNotificationTypes(input: request)
            .send(GetNotificationTypesPageResponse.self) { [weak self] response in
                self?.showLoading = false
                switch response {
                case .success(let typesPage):
                    guard let typesPage = typesPage else { return }
                    if typesPage.pageIndex == 1 {
                        self?.notificationTypes.removeAll()
                    }
                    self?.totalItemsCount = typesPage.totalItems
                    self?.notificationTypes.appendIfMissing(contentsOf: typesPage.data)
                    if typesPage.pageIndex == 1 {
                        self?.getDeactivatedNotificationTypes()
                    } else {
                        self?.setDisplayModels()
                    }
                case .failure(let error):
                    self?.showError = true
                    self?.errorText = error.localizedDescription
                }
            }
    }
    
    func getDeactivatedNotificationTypes() {
        let request = GetNotificationTypesRequest(pageIndex: 1)
        showLoading = true
        NotificationSettingsRouter.getDeactivatedNotificationTypes(input: request)
            .send(GetDeactivatedNotificationTypesPageResponse.self) { [weak self] response in
                self?.showLoading = false
                switch response {
                case .success(let deactivatedEventsPage):
                    guard let deactivatedEventsPage = deactivatedEventsPage else { return }
                    if deactivatedEventsPage.pageIndex == 1 {
                        self?.deactivatedEvents.removeAll()
                    }
                    self?.deactivatedEvents.appendIfMissing(contentsOf: deactivatedEventsPage.data)
                    self?.setDisplayModels()
                case .failure(let error):
                    self?.showError = true
                    self?.errorText = error.localizedDescription
                }
            }
    }
    
    func setDeacticatedNotificationTypes() {
        print("DEACTIVATED: \(deactivatedEvents)")
        let request = SetDeactivatedNotificationTypesRequest(ids: deactivatedEvents)
        showLoading = true
        NotificationSettingsRouter.setDeactivatedNotificationTypes(input: request)
            .send(Empty.self) { [weak self] response in
                self?.showLoading = false
                switch response {
                case .success:
                    break
                case .failure(let error):
                    self?.showError = true
                    self?.errorText = error.localizedDescription
                }
            }
    }
    
    // MARK: - Toggle methods
    func toggleType(id: String) {
        guard let typeModel = notificationTypesDisplayModels.first(where: { $0.id == id }),
              !typeModel.nonMandatoryEvents.isEmpty
        else {
            return
        }
        
        if let index = notificationTypesDisplayModels.firstIndex(of: typeModel) {
            if typeModel.state == .checked {
                // Uncheck non-mandatory events
                let eventIdsToDeactivate = typeModel.nonMandatoryEvents.map({ $0.id })
                if !eventIdsToDeactivate.isEmpty {
                    deactivatedEvents.appendIfMissing(contentsOf: eventIdsToDeactivate)
                }
                for (eventIndex, event) in notificationTypesDisplayModels[index].events.enumerated() where !event.isMandatory {
                    notificationTypesDisplayModels[index].events[eventIndex].isOn = false
                }
            } else {
                let eventIdsToActivate = typeModel.events.map({ $0.id })
                deactivatedEvents.removeIfExists(contentsOf: eventIdsToActivate)
                for (eventIndex, event) in notificationTypesDisplayModels[index].events.enumerated() where !event.isMandatory {
                    notificationTypesDisplayModels[index].events[eventIndex].isOn = true
                }
            }
            notificationTypesDisplayModels[index].state = notificationTypesDisplayModels[index].calculateState()
            // remove or add disabled valies in list depending on the new state
            setDeacticatedNotificationTypes()
        }
    }
    
    func toggleEvent(id: String) {
        if let index = notificationTypesDisplayModels.firstIndex(where: { $0.events.contains(where: { $0.id == id }) }),
           let eventIndex = notificationTypesDisplayModels[index].events.firstIndex(where: { $0.id == id }) {
            if deactivatedEvents.contains(id) {
                notificationTypesDisplayModels[index].events[eventIndex].isOn = true
                deactivatedEvents.removeIfExists(id)
            } else {
                notificationTypesDisplayModels[index].events[eventIndex].isOn = false
                deactivatedEvents.appendIfMissing(id)
            }
            notificationTypesDisplayModels[index].state = notificationTypesDisplayModels[index].calculateState()
            setDeacticatedNotificationTypes()
        }
    }
    
    // MARK: - Helpers
    private func setDisplayModels() {
        let mappedTypes = notificationTypes.map({ type -> NotificationTypeDisplayModel in
            let events = type.events.map({ event -> NotificationEventDisplayModel in
                return NotificationEventDisplayModel(id: event.id,
                                                     name: event.getLocalizedShortDescription(),
                                                     isOn: event.isMandatory ? true : !deactivatedEvents.contains(event.id),
                                                     isMandatory: event.isMandatory)
            })
            
            var typeModel = NotificationTypeDisplayModel(id: type.id,
                                                         name: type.getLocalizedName(),
                                                         events: events,
                                                         state: .unchecked)
            typeModel.state = typeModel.calculateState()
            return typeModel
        })
        
        notificationTypesDisplayModels = mappedTypes
    }
    
    func scrollViewBottomReached() {
        guard notificationTypes.count < totalItemsCount else { return }
        pageIndex += 1
        getNotificationTypes()
    }
    
    func reloadDataFromStart() {
        notificationTypes.removeAll()
        pageIndex = 1
        totalItemsCount = 0
        getNotificationTypes()
    }
}
