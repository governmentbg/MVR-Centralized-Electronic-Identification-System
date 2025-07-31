//
//  LogsFromMeViewModel.swift
//  eID-Mobile-iOS
//
//  Created by Sunay Gyultekin on 23.01.24.
//

import Foundation


final class LogsFromMeViewModel: LogsBaseViewModel {
    // MARK: Properties
    @Published var logs: [LogFromMe] = []
    
    // MARK: - API calls
    override func getLogs(fromStart: Bool = false) {
        if fromStart {
            cursor = nil
        }
        let request = LogsRequest(cursorSearchAfter: cursorSearchAfter,
                                  startDate: logRequestDate(date: startDateStr),
                                  endDate: logRequestDate(date: endDateStr),
                                  eventTypes: selectedTypes)
        showLoading = true
        LogsRouter.logsFromMe(input: request)
            .send(LogsFromMePageResponse.self) { [weak self] response in
                self?.showLoading = false
                switch response {
                case .success(let logsPage):
                    guard let logsPage = logsPage else { return }
                    if self?.cursor == nil {
                        self?.logs.removeAll()
                    }
                    if let cursor = logsPage.searchAfter.first {
                        self?.cursor = String(cursor)
                    }
                    self?.logs.appendIfMissing(contentsOf: logsPage.data)
                case .failure(let error):
                    self?.showError = true
                    self?.errorText = error.localizedDescription
                }
            }
    }
    
    /// MARK: Helpers
    override func reloadDataFromStart() {
        logs.removeAll()
        super.reloadDataFromStart()
    }
    
    override func scrollViewBottomReached() {
        guard logs.count >= 20 else { return }
        super.scrollViewBottomReached()
    }
}
