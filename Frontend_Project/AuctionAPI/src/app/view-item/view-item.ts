import { CommonModule } from "@angular/common";
import { Component, OnInit } from "@angular/core";
import { ActivatedRoute, Router } from "@angular/router";
import { FormsModule } from "@angular/forms";
import { NgChartsModule } from "ng2-charts";
import { ChartConfiguration } from "chart.js";
import { InfiniteScrollModule } from "ngx-infinite-scroll";
import { ItemService } from "../services/item.service";
import { ItemAllBids } from "../models/all-bids";
import { TokenService } from "../services/token.service";
import * as bootstrap from "bootstrap";
import { ImageService } from "../services/image.service";

@Component({
  selector: 'app-view-item',
  templateUrl: './view-item.html',
  styleUrls: ['./view-item.css'],
  standalone: true,
  imports: [CommonModule, FormsModule, NgChartsModule, InfiniteScrollModule]
})
export class ViewItem implements OnInit {
  item!: Item;
  bids: ItemAllBids[] = [];
  displayedBids: ItemAllBids[] = [];
  pageSize = 5;
  currentPage = 0;
  loadingMore = false;
  isAdmin = false;
  authorisedSeller = false;
  public fallbackImage: string = 'https://images.pexels.com/photos/414612/pexels-photo-414612.jpeg';
  imageUrl: string = '';
  showFullDescription: boolean = false;

  isLoading = true;
  errorMessage = '';
  highestAmount: number = 0;
  selectedImageUrl: string = '';

  chartData: ChartConfiguration<'line'>['data'] = {
    labels: [],
    datasets: [
      {
        data: [],
        label: 'Bid Amount',
        fill: false,
        borderColor: '#007bff',
        backgroundColor: 'rgba(5, 121, 244, 0.2)',
        tension: 0.3
      }
    ]
  };

  chartOptions = {
    responsive: true,
    maintainAspectRatio: false,
    plugins: {
      legend: {
        labels: {
          color: '#000',
          font: { size: 20 }
        }
      },
      tooltip: {
        titleFont: { size: 16 },
        bodyFont: { size: 14 }
      }
    },
    scales: {
      x: {
        ticks: { color: 'black', font: { size: 16 }, weight: 'bold' },
        title: {
          display: true,
          text: 'Bidder',
          color: '#000',
          font: { size: 20, weight: 'bold' }
        }
      },
      y: {
        ticks: { color: '#000', font: { size: 16 }, weight: 'bold' },
        title: {
          display: true,
          text: 'Bid Amount ($)',
          color: '#000',
          font: { size: 20, weight: 'bold' }
        }
      }
    }
  };

  constructor(
    private route: ActivatedRoute,
    private itemService: ItemService,
    private tokenService: TokenService,
    private router: Router,
    private imageService: ImageService
  ) {}

  ngOnInit(): void {
    const itemId = this.route.snapshot.paramMap.get('id')!;
    this.loadItem(itemId);
  }

  loadItem(id: string) {
    this.itemService.getItemById(id).subscribe({
      next: res => {
        this.item = res.data;
        this.isAdmin = this.tokenService.getRole()?.toLowerCase() === "admin";
        this.authorisedSeller = this.tokenService.getUserId() === this.item.sellerId;
        this.imageUrl = this.imageService.getItemImage(this.item.itemID);
        this.loadBids(this.item.itemID);
      },
      error: err => this.errorMessage = 'Failed to load item.'
    });
  }

  loadBids(id: string) {
    this.itemService.getAllBidsForItem(id).subscribe({
      next: res => {
        this.bids = res.data;
        this.updateHighestBid();
        this.updateChart();
        this.resetScroll();
        this.isLoading = false;
      },
      error: err => {
        this.errorMessage = err.error?.message || 'Error loading bids';
        this.isLoading = false;
      }
    });
  }

  updateHighestBid() {
    this.highestAmount = this.bids.length > 0 ? Math.max(...this.bids.map(b => b.amount)) : 0;
  }

  updateChart() {
    this.chartData = {
      labels: this.bids.map(b => b.name),
      datasets: [
        {
          data: this.bids.map(b => b.amount),
          label: 'Bid Amount',
          fill: false,
          borderColor: 'black',
          backgroundColor: '#007bff',
          tension: 0.3
        }
      ]
    };
  }

  resetScroll() {
    this.displayedBids = [];
    this.currentPage = 0;
    this.loadNextPage();
  }

  loadNextPage() {
    const start = this.currentPage * this.pageSize;
    const next = this.bids.slice(start, start + this.pageSize);
    this.displayedBids = [...this.displayedBids, ...next];
    this.currentPage++;
  }

  onScrollDown() {
    if (this.displayedBids.length < this.bids.length && !this.loadingMore) {
      this.loadingMore = true;
      setTimeout(() => {
        this.loadNextPage();
        this.loadingMore = false;
      }, 1000);
    }
  }

  onPlaceBid(): void {
    this.router.navigate(['/items/live-bid', this.item.itemID]);
  }

  exportCSV() {
    const csvRows = [
      ['Name', 'Amount', 'Date', 'Time'],
      ...this.bids.map(b => [b.name, b.amount, new Date(b.bid_timestamp).toLocaleString()])
    ];
    const csv = csvRows.map(row => row.join(',')).join('\n');
    const blob = new Blob([csv], { type: 'text/csv' });
    const url = window.URL.createObjectURL(blob);
    const a = document.createElement('a');
    a.href = url;
    a.download = 'bids.csv';
    a.click();
  }

  openImageModal(url: string) {
    this.selectedImageUrl = url;
    const modal = new bootstrap.Modal(document.getElementById('imageModal')!);
    modal.show();
  }
}
