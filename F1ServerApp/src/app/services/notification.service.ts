import { Injectable } from '@angular/core';
import { MatSnackBar } from '@angular/material/snack-bar';

@Injectable({ providedIn: 'root' })

export class NotificationService
{
  private readonly durationMs: number = 5000;

  // Constructor
  constructor(private readonly snackBar: MatSnackBar)
  {
  }

  // Show an error message to the user
  public showError(message: string): void
  {
    this.snackBar.open(message, 'Dismiss', { duration: this.durationMs, panelClass: ['notification-error'] });
  }
}
