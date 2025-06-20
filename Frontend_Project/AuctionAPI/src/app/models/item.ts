interface Item {
  itemID: string;
  title: string;
  description: string;
  status: string;
  category: string;
  startingPrice: number;
  startDate: string;
  endDate: string;
  currentBid?: number;
  currentBidderName?: string;
  sellerName: string;
  imageUrl?: string;
}