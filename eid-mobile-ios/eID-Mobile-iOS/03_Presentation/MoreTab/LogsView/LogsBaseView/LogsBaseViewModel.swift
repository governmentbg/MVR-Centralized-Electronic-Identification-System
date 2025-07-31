//
//  LogsBaseViewModel.swift
//  eID-Mobile-iOS
//
//  Created by Teodora Georgieva on 16.10.24.
//

import SwiftUI


class LogsBaseViewModel: ObservableObject, APICallerViewModel {
    // MARK: - Properties
    @Published var logsDescriptions = [LocalisedLog]()
    @Published var showLoading: Bool = false
    @Published var showError: Bool = false
    @Published var errorText: String = ""
    @Published var showFilter: Bool = false
    @Published var cursor: String? = nil
    @Published var viewState: ViewState = .fromMe
    /// Request params
    @Published var type: [String] = []
    @Published var startDateStr: String = ""
    @Published var endDateStr: String = ""
    /// Computed
    @Published var isFilterApplied: Bool = false
    var screenTitle: String {
        return viewState == .fromMe ? "logs_from_me_screen_title".localized() : "logs_to_me_screen_title".localized()
    }
    var cursorSearchAfter: [String] {
        guard let cursor = cursor else { return [] }
        return [cursor]
    }
    var selectedTypes: [String]? {
        if type.isEmpty {
            return nil
        } else {
            var types = [String]()
            for element in type.map({ $0.trimmingCharacters(in: .whitespaces) }) {
                if let logKey = logsDescriptions.first(where: { $0.localisedDescription == element })?.key {
                    types.append(logKey)
                }
            }
            return types
        }
    }
    /// API responses
    @Published private var pageIndex = 0
    @Published private var totalItemsCount = 0
    
    /// Init
    init(viewState: ViewState,
         logsDescriptions: [LocalisedLog]) {
        self.viewState = viewState
        self.logsDescriptions = logsDescriptions
    }
    
    // MARK: - API calls
    func getLogs(fromStart: Bool = false) { }
    
    // MARK: - Helpers
    func reloadDataFromStart() {
        pageIndex = 1
        totalItemsCount = 0
        getLogs(fromStart: true)
        checkFilter()
    }
    
    func scrollViewBottomReached() {
        getLogs()
    }
    
    func logRequestDate(date: String) -> String? {
        return date.toDate(withFormats: [.iso8601Date])?.endOfDay.normalizeDate(outputFormat: .iso8601Date) ?? nil
    }
    
    func getEventTitle(log: any Logs) -> String {
        return logsDescriptions.first(where: { $0.key == log.eventType })?.localisedDescription ?? log.eventType
    }
    
    private func checkFilter() {
        isFilterApplied = !type.isEmpty || !startDateStr.isEmpty || !endDateStr.isEmpty
    }
}

// MARK: - Enums
extension LogsBaseViewModel {
    enum ViewState {
        case fromMe
        case toMe
    }
}
