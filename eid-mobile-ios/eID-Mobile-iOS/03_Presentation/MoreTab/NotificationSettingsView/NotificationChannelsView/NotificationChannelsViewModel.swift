//
//  NotificationChannelsViewModel.swift
//  eID-Mobile-iOS
//
//  Created by Sunay Gyultekin on 11.10.23.
//

import Foundation
import Alamofire


final class NotificationChannelsViewModel: ObservableObject, APICallerViewModel {
    // MARK: - Properties
    private var channels: [NotificationChannelResponse] = []
    private var selectedChannels: [SelectedNotificationChannelResponse] = []
    @Published var channelsDisplayModels: [NotificationChannelDisplayModel] = []
    @Published var showLoading: Bool = false
    @Published var showError: Bool = false
    @Published var errorText: String = ""
    
    // MARK: - API calls
    func getNotificationChannels() {
        let request = GetNotificationChannelsRequest(channelName: "", pageIndex: 1)
        showLoading = true
        NotificationSettingsRouter.getNotificationChannels(input: request)
            .send(GetNotificationChannelsPageResponse.self) { [weak self] response in
                self?.showLoading = false
                switch response {
                case .success(let channelsPage):
                    guard let channelsPage = channelsPage else { return }
                    self?.channels.appendIfMissing(contentsOf: channelsPage.data)
                    self?.getSelectedNotificationChannels()
                case .failure(let error):
                    self?.showError = true
                    self?.errorText = error.localizedDescription
                }
            }
    }
    
    func getSelectedNotificationChannels() {
        let request = GetNotificationChannelsRequest(pageIndex: 1)
        showLoading = true
        NotificationSettingsRouter.getSelectedNotificationChannels(input: request)
            .send(GetSelectedNotificationChannelsPageResponse.self) { [weak self] response in
                guard let self = self else { return }
                self.showLoading = false
                switch response {
                case .success(let selectedChannelsPage):
                    guard let selectedChannelsPage = selectedChannelsPage else { return }
                    let filteredChannels = selectedChannelsPage.data.filter({ self.channels.map({ $0.id }).contains($0) })
                    self.selectedChannels.appendIfMissing(contentsOf: filteredChannels)
                    self.updateDisplayModels()
                case .failure(let error):
                    self.showError = true
                    self.errorText = error.localizedDescription
                }
            }
    }
    
    private func setSelectedNotificationChannels(id: String, isOn: Bool) {
        var channelsToUpdate = selectedChannels
        if isOn {
            channelsToUpdate.appendIfMissing(id)
        } else {
            channelsToUpdate.removeIfExists(id)
        }
        
        let request = SetSelectedNotificationChannelsRequest(ids: channelsToUpdate)
        showLoading = true
        NotificationSettingsRouter.setSelectedNotificationChannels(input: request)
            .send(Empty.self) { [weak self] response in
                self?.showLoading = false
                switch response {
                case .success:
                    self?.selectedChannels = channelsToUpdate
                case .failure(let error):
                    self?.showError = true
                    self?.errorText = error.localizedDescription
                }
                
                self?.updateDisplayModels()
            }
    }
    
    // MARK: - Helpers
    private func updateDisplayModels() {
        let mappedChannels = channels.map({ channel -> NotificationChannelDisplayModel in
            return NotificationChannelDisplayModel(id: channel.id,
                                                   name: channel.getLocalizedName(),
                                                   description: channel.getLocalizedDescription(),
                                                   isSelected: selectedChannels.contains(channel.id),
                                                   isMandatory: channel.name == "SMTP")
        })
        channelsDisplayModels = mappedChannels
    }
    
    func toggleChannel(id: String, isOn: Bool) {
        setSelectedNotificationChannels(id: id, isOn: isOn)
    }
}
