//
//  EIDTimer.swift
//  eID-Mobile-iOS
//
//  Created by Teodora Georgieva on 3.06.24.
//

import Foundation


class EIDTimer: ObservableObject {
    // MARK: - Properties
    var timerFinished: () -> ()
    private var timer: Timer?
    private var repeats: Bool
    
    // MARK: - Init
    init(Δt: Double, repeats: Bool? = nil, timerFinished: @escaping () -> ()) {
        self.timerFinished = timerFinished
        self.repeats = repeats ?? false
        startTimer(Δt: Δt)
    }
    
    // MARK: - Public methods
    func restart(Δt: Double){
        kill()
        startTimer(Δt: Δt)
    }
    
    func kill() {
        timer?.invalidate()
        timer = nil
    }
    
    // MARK: - Private methods
    private func startTimer(Δt: Double) {
        timer = Timer.scheduledTimer(withTimeInterval: Δt, repeats: repeats) { [weak self] _ in
            self?.timerFinished()
        }
    }
}
