import { CommonModule } from '@angular/common';
import { Component, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ScannedunitService } from '../../_services/scannedunit.service';
@Component({
  selector: 'app-scanmainserial',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './scanmainserial.html',
  styleUrls: ['./scanmainserial.css'],
})
export class ScanmainserialComponent {
  private scannedService = inject(ScannedunitService);
  serialInput = signal('');
  onScanEnter() {
    const serial = this.serialInput().trim();
    if (!serial) return;
    this.scannedService.create(serial).subscribe(() => {
      this.serialInput.set('');
    });
  }
}
