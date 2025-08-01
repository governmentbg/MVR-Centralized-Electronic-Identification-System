//
//  HostingView.swift
//  eID-Mobile-iOS
//
//  Created by Teodora Georgieva on 12.07.24.
//

import SwiftUI


class HostingView<T: View>: UIView {
    // MARK: - Properties
    private(set) var hostingController: UIHostingController<T>
    var rootView: T {
        get { hostingController.rootView }
        set { hostingController.rootView = newValue }
    }
    
    // MARK: - Init
    init(rootView: T, frame: CGRect = .zero) {
        hostingController = UIHostingController(rootView: rootView)
        super.init(frame: frame)
        
        backgroundColor = .clear
        hostingController.view.backgroundColor = backgroundColor
        hostingController.view.frame = self.bounds
        hostingController.view.autoresizingMask = [.flexibleWidth, .flexibleHeight]
        addSubview(hostingController.view)
    }
    
    required init?(coder: NSCoder) {
        fatalError("init(coder:) has not been implemented")
    }
}
