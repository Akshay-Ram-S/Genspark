import { CommonModule } from "@angular/common";
import { Component, OnInit } from "@angular/core";
import { ActivatedRoute } from "@angular/router";
import { FormsModule } from "@angular/forms";
import { NgChartsModule, NgChartsConfiguration } from "ng2-charts";
import { ChartConfiguration } from "chart.js";
import { InfiniteScrollModule } from "ngx-infinite-scroll";

import { ItemService } from "../services/item.service";
import { ItemAllBids } from "../models/all-bids";
import { ItemComponent } from "../item-component/item-component";

@Component({
  selector: 'app-view-item',
  templateUrl: './view-item.html',
  styleUrls: ['./view-item.css'],
  standalone: true,
  imports: [CommonModule, ItemComponent, FormsModule, NgChartsModule, InfiniteScrollModule]
})
export class ViewItem implements OnInit {
  item!: Item;
  bids: ItemAllBids[] = [];
  displayedBids: ItemAllBids[] = [];
  pageSize = 5;
  currentPage = 0;
  loadingMore = false;


  isLoading = true;
  errorMessage = '';
  highestAmount: number = 0;
  sortOption = '';
  bidderFilter = '';

  chartData: ChartConfiguration<'line'>['data'] = {
    labels: [],
    datasets: [
      {
        data: [],
        label: 'Bid Amount',
        fill: false,
        borderColor: 'beige',
        backgroundColor: 'goldenrod',
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
        ticks: { color: 'black', font: { size: 16 }, weight: 'bold'},
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

  constructor(private route: ActivatedRoute, private itemService: ItemService) {}

  ngOnInit(): void {
    const itemId = this.route.snapshot.paramMap.get('id')!;
    this.loadItem(itemId);
    this.loadBids(itemId);
  }

  loadItem(id: string) {
    this.itemService.getItemById(id).subscribe({
      next: res => this.item = res.data,
      error: err => this.errorMessage = 'Failed to load item.'
    });
  }

  loadBids(id: string) {
    this.itemService.getAllBidsForItem(id).subscribe({
      next: res => {
        this.bids = res.data;
        this.updateHighestBid();
        this.sortBids();
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

  resetScroll() {
    this.displayedBids = [];
    this.currentPage = 0;
    this.loadNextPage();
  }

  loadNextPage() {
    const filtered = this.filteredBids;
    const start = this.currentPage * this.pageSize;
    const next = filtered.slice(start, start + this.pageSize);
    this.displayedBids = [...this.displayedBids, ...next];
    this.currentPage++;
  }

  onScrollDown() {
    if (this.displayedBids.length < this.filteredBids.length && !this.loadingMore) {
      this.loadingMore = true;

      setTimeout(() => {
        this.loadNextPage();
        this.loadingMore = false;
      }, 1000); 
    }
  }

  sortBids() {
    if (this.sortOption === 'highest') {
      this.bids.sort((a, b) => b.amount - a.amount);
    } else if (this.sortOption === 'lowest') {
      this.bids.sort((a, b) => a.amount - b.amount);
    } else {
      this.bids.sort((a, b) => new Date(b.bid_timestamp).getTime() - new Date(a.bid_timestamp).getTime());
    }
    this.updateChart();
    this.resetScroll();
  }

  get filteredBids() {
    return this.bids.filter(b => b.name.toLowerCase().includes(this.bidderFilter.toLowerCase()));
  }

  updateChart() {
    const filtered = this.filteredBids;
    this.chartData = {
      labels: filtered.map(b => b.name),
      datasets: [
        {
          data: filtered.map(b => b.amount),
          label: 'Bid Amount',
          fill: false,
          borderColor: 'beige',
          backgroundColor: 'goldenrod',
          tension: 0.3
        }
      ]
    };
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

  onFilterChange() {
    this.updateChart();
    this.resetScroll();
  }
}
