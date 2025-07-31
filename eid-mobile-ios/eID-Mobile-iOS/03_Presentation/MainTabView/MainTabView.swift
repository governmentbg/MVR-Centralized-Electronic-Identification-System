//
//  MainTabView.swift
//  eID-Mobile-iOS
//
//  Created by Sunay Gyultekin on 10.10.23.
//

import SwiftUI


struct MainTabView: View {
    // MARK: - Properties
    private let tabs = EIDTabItems.allCases
    private var singleTabWidth: CGFloat {
        return UIScreen.main.bounds.width/CGFloat(tabs.count)
    }
    @AppStorage(Constants.UserDefaultsKeys.selectedTab) private var selectedTab: Int = EIDTabItems.home.rawValue
    
    // MARK: - Body
    var body: some View {
        ZStack(alignment: .bottomLeading) {
            TabView(selection: $selectedTab) {
                ForEach(tabs, id: \.self) { tab in
                    tabContent(for: tab)
                        .tabItem {
                            EIDTabItem(iconName: tab.iconName,
                                       title: tab.title)
                        }
                        .tag(tab.rawValue)
                }
            }
            Rectangle()
                .offset(x: singleTabWidth * CGFloat(selectedTab),
                        y: 2)
                .frame(width: singleTabWidth,
                       height: 2)
                .foregroundStyle(Color.themeSecondaryAlert)
        }
        .setupTabBar(itemWidth: singleTabWidth,
                     height: 48)
        .background(Color.themeSecondaryLight)
    }
    
    // MARK: - Helpers
    private func tabContent(for tab: EIDTabItems) -> AnyView {
        switch tab {
        case .home:
            return AnyView(HomeView())
        case .electronicIdentity:
            return AnyView(EIDManagementView())
        case .pending:
            return AnyView(PendingView())
        case .more:
            return AnyView(MoreView())
        }
    }
}


// MARK: - Preview
#Preview {
    MainTabView()
}
