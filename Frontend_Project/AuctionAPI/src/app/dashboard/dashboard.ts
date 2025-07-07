import { Component, OnInit, AfterViewInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { SellerService } from '../services/seller.service';
import { BidderService } from '../services/bidder.service';
import { ItemService } from '../services/item.service';
import Chart from 'chart.js/auto';
import { Router, RouterLink } from '@angular/router';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './dashboard.html',
  styleUrl: './dashboard.css'
})
export class Dashboard implements OnInit, AfterViewInit {
  sellerCount: number = 0;
  bidderCount: number = 0;
  totalItems: number = 0;
  activeItems: number = 0;
  soldItems: number = 0;
  unsoldItems: number = 0;
  allItems: any;
  sellers: any;
  bidders: any;

  constructor(
    private sellerService: SellerService,
    private bidderService: BidderService,
    private itemService: ItemService,
    private router: Router
  ) {}

  ngOnInit() {
    this.loadCounts();
  }

  ngAfterViewInit() {
    setTimeout(() => this.renderChart(), 500); 
  }


  goTo(type: 'bidders' | 'sellers' | 'items') {
    if (type === 'bidders') {
      this.router.navigate(['/users/bidder']);
    } else if (type === 'sellers') {
      this.router.navigate(['/users/seller']);
    }
    else if(type === 'items'){
      this.router.navigate(['/items'])
    }
  }


  loadCounts() {
    this.sellerService.getSellers().subscribe(
      res => {
      this.sellers = res.data;
      this.sellerCount = this.sellers.length;
    });

    this.bidderService.getBidders().subscribe(
      res => {
      this.bidders = res.data;
      this.bidderCount = this.bidders.length;
    });

    this.itemService.getFilteredItems({ page: 1, pageSize: 1000 }).subscribe(res => {
      const items = res.data || [];
      this.allItems = items;
      this.totalItems = items.length;
      this.soldItems = items.filter(item => item.status === 'Sold').length;
      this.unsoldItems = items.filter(item => item.status === 'Unsold').length;
      this.activeItems = items.filter(item => item.status === 'Active').length;

      this.renderChart();
    });
  }

  private chartInstance: Chart | null = null;

  renderChart() {
    const canvas = document.getElementById('statusChart') as HTMLCanvasElement;
    if (!canvas) return;

    if (this.chartInstance) {
      this.chartInstance.destroy();
    }

    this.chartInstance = new Chart(canvas, {
      type: 'doughnut',
      data: {
        labels: ['Active', 'Sold', 'Unsold'],
        datasets: [
          {
            data: [this.activeItems, this.soldItems, this.unsoldItems ],
            backgroundColor: ['green', 'blue', 'grey'],
            borderWidth: 1,
          },
        ],
      },
      options: {
        responsive: true,
        plugins: {
          legend: {
            position: 'bottom',
            labels: { color: '#333' }
          }
        }
      },
    });
  }

  downloadItemsCSV(status: 'Sold' | 'Unsold' | 'Active' | 'All'): void {
    if (!this.allItems) return;

    const headers = [
      'Item ID',
      'Title',
      'Category',
      'Starting Price',
      'Current Bid',
      'End Date',
      'Seller',
      'Buyer',
      'Status'
    ];

    let filteredItems = this.allItems;

    if (status !== 'All') {
      filteredItems = filteredItems.filter((item: any) => item.status === status);
    }

    const rows = filteredItems.map((item: any) => [
      item.itemID,
      item.title,
      item.category,
      item.startingPrice,
      item.currentBid ?? '-',
      new Date(item.endDate).toLocaleString(),
      item.sellerName,
      item.boughtBy,
      item.status
    ]);

    const csvContent = [headers, ...rows]
      .map(row => row.map((val: any) => `"${val}"`).join(','))
      .join('\n');

    const blob = new Blob([csvContent], { type: 'text/csv;charset=utf-8;' });
    const url = URL.createObjectURL(blob);
    const link = document.createElement('a');

    link.setAttribute('href', url);
    link.setAttribute('download', `${status.toLowerCase()}-items.csv`);
    link.style.visibility = 'hidden';

    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
  }


  downloadUsersCSV(type: 'seller' | 'bidder'): void {
    const headers = ['ID', 'Name', 'Email', 'Phone'];

    const users = type === 'seller' ? this.sellers : this.bidders;

    if (!users || users.length === 0) {
      alert(`No ${type}s found to export.`);
      return;
    }

    const rows = users.map((user: any) => [
      user.id || user.sellerId || user.bidderId || '-',
      user.user.name,
      user.user.email,
      user.user.phone || '-'
    ]);

    const csvContent = [headers, ...rows]
      .map(row => row.map((cell:any) => `"${cell}"`).join(','))
      .join('\n');

    const blob = new Blob([csvContent], { type: 'text/csv;charset=utf-8;' });
    const url = URL.createObjectURL(blob);
    const link = document.createElement('a');

    link.setAttribute('href', url);
    link.setAttribute('download', `${type}-list.csv`);
    link.style.visibility = 'hidden';

    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
  }



}