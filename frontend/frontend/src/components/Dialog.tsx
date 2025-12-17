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
} from "@/components/ui/select"
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { useState } from "react";
import { getAccessToken } from "@/utils/auth";
import { apiFetch } from "@/lib/apiFetch";
const API_BASE = "http://localhost:5051";

type Props = {
  onAssetCreated: () => void;
  open,
  onOpenChange: (open: boolean) => void;
};
export function AddAssetDialog({ open, onOpenChange, onAssetCreated }: Props) {
  const [loading, setLoading] = useState(false);

  // Form data
  const [name, setName] = useState("");
  const [category, setCategory] = useState("");
  const [value, setValue] = useState<number | "">("");
  const [purchaseDate, setPurchaseDate] = useState("");
  const [vendorDetails, setVendorDetails] = useState("");
  const [condition, setCondition] = useState("");
  const [status, setStatus] = useState<
    "Available" | "Assigned" | "Maintenance"
  >("Available");

  const close = () => {
    onOpenChange(false);
    setName("");
    setCategory("");
    setPurchaseDate("");
    setVendorDetails("");
    setCondition("");
    setStatus("Available");
  };
  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setLoading(true);
    try {
      const res = await apiFetch(`${API_BASE}/api/assets`, {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
        },
        body: JSON.stringify({
          name,
          category,
          purchaseDate,
          vendor: vendorDetails,
          condition,
          status,
        }),
      });

      if (!res.ok) throw new Error("Failed to create asset");

      onAssetCreated();
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
            <DialogTitle>Create Asset</DialogTitle>
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
              <Label>Product Condition</Label>
              <Input
                placeholder="e.g. New / Used / Faulty"
                value={condition}
                onChange={(e) => setCondition(e.target.value)}
                required
              />
            </div>
            <div className="grid gap-2">
              <Label>Status</Label>
              <Select value={status} onValueChange={(v) => setStatus(v as "Available" | "Assigned" | "Maintenance")}>
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
              variant="outline"
            >
              Cancel
            </Button>

            <Button type="submit" className="bg-blue-500 hover:bg-blue-300">
              Create Asset
            </Button>
          </DialogFooter>
        </form>
      </DialogContent>
    </Dialog>
  );
}
