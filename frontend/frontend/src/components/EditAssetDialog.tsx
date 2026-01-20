import { Button } from "@/components/ui/button";
import {
  Dialog,
  DialogClose,
  DialogContent,
  DialogDescription,
  DialogFooter,
  DialogHeader,
  DialogTitle,
  DialogTrigger,
} from "@/components/ui/dialog";
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/components/ui/select";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { useState, useEffect } from "react";
import { api } from "@/client";

type Props = {
  asset: any;
  open: boolean;
  onOpenChange: (open: boolean) => void;
  onAssetUpdated: () => void;
};
export function EditAssetDialog({
  asset,
  onAssetUpdated,
  onOpenChange,
  open,
}: Props) {
  const [loading, setLoading] = useState(false);

  // Form data
  const [name, setName] = useState("");
  const [category, setCategory] = useState("");
  const [purchaseDate, setPurchaseDate] = useState("");
  const [vendorDetails, setVendorDetails] = useState("");
  const [condition, setCondition] = useState("");
  const [status, setStatus] = useState<
    "Available" | "Assigned" | "Maintenance"
  >("Available");
  const [model, setModel] = useState("");
  const [barcode, setBarcode] = useState("");
  const [imageUrl, setImageUrl] = useState("");
  useEffect(() => {
    if (!asset) return;
    setName(asset.name);
    setCategory(asset.category);

    // get only date from purchasedate
    if (asset.purchaseDate) {
      setPurchaseDate(String(asset.purchaseDate).slice(0, 10)); // "YYYY-MM-DD"
    } else {
      setPurchaseDate("");
    }
    setVendorDetails(asset.vendor);
    setCondition(asset.condition);
    setStatus(asset.status);
    setModel(asset.model);
    setBarcode(asset.barcode || "");
    setImageUrl(asset.imageUrl || "");
  }, [asset, open]);

  const close = () => {
    onOpenChange(false);
  };
  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setLoading(true);
    try {
      await api.put(`/api/assets/${asset.id}`, {
        name,
        category,
        purchaseDate,
        vendor: vendorDetails,
        condition,
        status,
        model,
        barcode,
        imageUrl,
      });

      onAssetUpdated();
      close();
    } catch (err) {
      console.error(err);
    } finally {
      setLoading(false);
    }
  };

  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent className="sm:max-w-[425px]">
        <form onSubmit={handleSubmit}>
          <DialogHeader>
            <DialogTitle>Edit Asset</DialogTitle>
            <DialogDescription>
              Add a new asset to your portfolio.
            </DialogDescription>
          </DialogHeader>

          <div className="grid gap-4 py-4">
            <div className="grid gap-2">
              <Label>Name</Label>
              <Input
                placeholder="e.g. Plates"
                value={name}
                onChange={(e) => setName(e.target.value)}
                required
              />
            </div>

            <div className="grid gap-2">
              <Label>Category</Label>
              <Input
                placeholder="e.g. Kitchenware"
                value={category}
                onChange={(e) => setCategory(e.target.value)}
                required
              />
            </div>
            <div className="grid gap-2">
              <Label>Purchase Date</Label>
              <Input
                type="date"
                value={purchaseDate}
                onChange={(e) => setPurchaseDate(e.target.value)}
                required
              />
            </div>
            <div className="grid gap-2">
              <Label>Vendor Details</Label>
              <Input
                placeholder="e.g. Maxwell Williams"
                value={vendorDetails}
                onChange={(e) => setVendorDetails(e.target.value)}
                required
              />
            </div>
            <div className="grid gap-2">
              <Label>Model</Label>
              <Input
                placeholder="e.g. MBP-2021"
                value={model}
                onChange={(e) => setModel(e.target.value)}
                required
              />
            </div>
            <div className="grid gap-2">
              <Label>Product Condition</Label>
              <Input
                placeholder="e.g. New / Used / Faulty"
                value={condition}
                onChange={(e) => setCondition(e.target.value)}
                required
              />
            </div>
            <div className="grid gap-2">
              <Label>Image URL</Label>
              <Input
                value={imageUrl}
                onChange={(e) => setImageUrl(e.target.value)}
              />
            </div>
            <div className="grid gap-2">
              <Label>Barcode</Label>
              <Input
                value={barcode}
                onChange={(e) => setBarcode(e.target.value)}
              />
            </div>

            <div className="grid gap-2">
              <Label>Status</Label>
              <Select
                value={status}
                onValueChange={(v) =>
                  setStatus(v as "Available" | "Assigned" | "Maintenance")
                }
              >
                <SelectTrigger>
                  <SelectValue />
                </SelectTrigger>
                <SelectContent>
                  <SelectItem value="Available">Available</SelectItem>
                  <SelectItem value="Assigned">Assigned</SelectItem>
                  <SelectItem value="Maintenance">Maintenance</SelectItem>
                </SelectContent>
              </Select>
            </div>
          </div>

          <DialogFooter>
            <Button
              type="button"
              onClick={close}
              className="bg-white text-gray hover:bg-gray-100"
            >
              Cancel
            </Button>

            <Button type="submit" className="bg-blue-500 hover:bg-blue-300">
              Save Changes
            </Button>
          </DialogFooter>
        </form>
      </DialogContent>
    </Dialog>
  );
}
